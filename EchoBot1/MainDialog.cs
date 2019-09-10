using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot1
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog() : base(nameof(MainDialog))
        {
            var steps = new WaterfallStep[]
            {
                GreetingAsync,
                PromptEnterNameAsync,
                PostPromptEnterNameAsync,
                PromptEnterAgeAsync,
                PostPromptEnterAgeAsync,
                PromptEnterChoiceAsync,
                PostPromptEnterChoiceAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), steps));
            AddDialog(new TextPrompt(nameof(TextPrompt), textPromtValidator));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), numberPromptValidator));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GreetingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Hello!"), cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> PromptEnterNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt),
           new PromptOptions
           {
               Prompt = MessageFactory.Text("Give me your name")
           }, cancellationToken);
        }

        private async Task<DialogTurnResult> PostPromptEnterNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Name"] = (string)stepContext.Result;

            return await stepContext.NextAsync(cancellationToken: cancellationToken);

        }

        private async Task<bool> textPromtValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            const string hardcodedName = "David Attenborough";

            if (!string.IsNullOrWhiteSpace(promptContext.Recognized.Value))

            {

                if (hardcodedName.Equals(promptContext.Recognized.Value.Trim(), StringComparison.InvariantCultureIgnoreCase))

                {

                    await promptContext.Context.SendActivityAsync("Sir David, it's an honor to have you here!", cancellationToken: cancellationToken);

                    return true;

                }

            }

            await promptContext.Context.SendActivityAsync("I was expecting someone else. Anyway, I'll be here for you nonetheless.", cancellationToken: cancellationToken);

            return true;
        }        

        private async Task<DialogTurnResult> PromptEnterAgeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
           new PromptOptions
           {
               Prompt = MessageFactory.Text("Give me your age"),
               RetryPrompt=MessageFactory.Text("You must enter valid age")
           }, cancellationToken);
        }

        private async Task<DialogTurnResult> PostPromptEnterAgeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Age"] = (int)stepContext.Result;

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<bool> numberPromptValidator(PromptValidatorContext<int> promptContext, CancellationToken cancellationToken)
        {
           return promptContext.Recognized.Value > 18 && promptContext.Recognized.Value < 100;
        }


        private async Task<DialogTurnResult> PromptEnterChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string user = $"Name is {(string)stepContext.Values["Name"]} age is {(int)stepContext.Values["Age"]}.";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You are {user}"));
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Is it true?"),
                Choices=ChoiceFactory.ToChoices(new List<string> { "true", "false"})
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> PostPromptEnterChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (bool.TryParse(((FoundChoice)stepContext.Result).Value, out var confirmation))
            {

                if (confirmation)
                {

                    //finally save the bot state in the accessors

                    //var botState = stepContext.Values[BotState.StateKey] as BotState;

                    //await this.accessors.BotState.SetAsync(stepContext.Context, botState, cancellationToken);

                    await stepContext.Context.SendActivityAsync("Perfect! I have recorded your entry. Goodbye!", cancellationToken: cancellationToken);

                    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

                }

            }

            //in case something went wrong, we start all over again

            await stepContext.Context.SendActivityAsync("Oh, that's not right. Let's try again.", cancellationToken: cancellationToken);

            return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), cancellationToken: cancellationToken);
            //return await stepContext.NextAsync(null,cancellationToken);
        }

        

       

        



    }
}
