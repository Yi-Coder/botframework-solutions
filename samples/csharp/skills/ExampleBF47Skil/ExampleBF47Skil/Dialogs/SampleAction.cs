﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Solutions.Responses;
using ExampleBF47Skill.Responses.Sample;
using ExampleBF47Skill.Services;
using Newtonsoft.Json;

namespace ExampleBF47Skill.Dialogs
{
    public class SampleActionInput
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class SampleActionOutput
    {
        [JsonProperty("customerId")]
        public int CustomerId { get; set; }
    }

    public class SampleAction : SkillDialogBase
    {
        public SampleAction(
            BotSettings settings,
            BotServices services,
            ResponseManager responseManager,
            ConversationState conversationState,
            IBotTelemetryClient telemetryClient)
            : base(nameof(SampleAction), settings, services, responseManager, conversationState, telemetryClient)
        {
            var sample = new WaterfallStep[]
            {               
                PromptForName,
                GreetUser,
                End,
            };

            AddDialog(new WaterfallDialog(nameof(SampleAction), sample));
            AddDialog(new TextPrompt(DialogIds.NamePrompt));

            InitialDialogId = nameof(SampleAction);
        }

        private async Task<DialogTurnResult> PromptForName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If we have been provided a input data structure we pull out provided data as appropriate
            // and make a decision on whether the dialog needs to prompt for anything.
            var actionInput = stepContext.Options as SampleActionInput;
            if (actionInput != null && !string.IsNullOrEmpty(actionInput.Name))
            {               
                // We have Name provided by the caller so we skip the Name prompt.
                return await stepContext.NextAsync(actionInput.Name);
            }

            var prompt = ResponseManager.GetResponse(SampleResponses.NamePrompt);
            return await stepContext.PromptAsync(DialogIds.NamePrompt, new PromptOptions { Prompt = prompt });
        }

        private async Task<DialogTurnResult> GreetUser(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokens = new StringDictionary
            {
                { "Name", stepContext.Result.ToString() },
            };

            var response = ResponseManager.GetResponse(SampleResponses.HaveNameMessage, tokens);
            await stepContext.Context.SendActivityAsync(response);

            // Pass the response which we'll return to the user onto the next step
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> End(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Simulate a response object payload
            SampleActionOutput actionResponse = new SampleActionOutput();
            actionResponse.CustomerId = new Random().Next();       

            // We end the dialog (generating an EndOfConversation event) which will serialize the result object in the Value field of the Activity
            return await stepContext.EndDialogAsync(actionResponse);
        }

        private class DialogIds
        {
            public const string NamePrompt = "namePrompt";
        }
    }
}
