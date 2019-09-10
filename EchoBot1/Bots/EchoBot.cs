// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace EchoBot1.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Equals("hero"))
            {
                await GetHeroCard(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Text.Equals("weather"))
            {
                await GetWeatherCard(turnContext, cancellationToken);
            }
            else if(turnContext.Activity.Text.Equals("adaptive"))
            {
                await GetAdaptiveCard(turnContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text} {turnContext.Activity.Text.Length} {DateTime.Now}"), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }

        private async Task GetHeroCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var heroCard = new HeroCard
            {
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.PostBack,"Send greeteng.", value:"Hello bot!"),
                    new CardAction(ActionTypes.OpenUrl,"Go to google.", value:"https://google.com")
                },
                Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Alt="A cat",
                        Url="https://cdn.hswstatic.com/gif/abyssinian-cat.jpg",
                        Tap=new CardAction(ActionTypes.ShowImage,"A cat again",value:"https://cdn.hswstatic.com/gif/abyssinian-cat.jpg")
                    }
                }
            };

            var message = MessageFactory.Attachment(heroCard.ToAttachment(), "A cat 2");

            await turnContext.SendActivityAsync(message);
        }

        private async Task GetWeatherCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var weatherJson = await File.ReadAllTextAsync(@".\weather_card.json", cancellationToken);

            var weatherCard = new Attachment()
            {

                ContentType = "application/vnd.microsoft.card.adaptive",

                Content = JsonConvert.DeserializeObject(weatherJson),

            };

            var message = MessageFactory.Attachment(weatherCard);

            await turnContext.SendActivityAsync(message);
        }
        private async Task GetAdaptiveCard(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var adaptiveCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 1));
            adaptiveCard.Body.Add(new AdaptiveTextBlock
            {
                Text = "A cat with a tie, again.",

                Size = AdaptiveTextSize.Large,

                Weight = AdaptiveTextWeight.Bolder,

            });

            adaptiveCard.Body.Add(new AdaptiveImage
            {
                AltText = "A cat with a tie, again",

                UrlString = "https://aka.ms/catwithtie",

                SelectAction = new AdaptiveOpenUrlAction

                {

                    UrlString = "https://aka.ms/catwithtie",

                }
            });

            adaptiveCard.Actions.Add(new AdaptiveOpenUrlAction

            {

                Title = "Go to a website",

                UrlString = "https://www.microsoft.com"

            });

            adaptiveCard.Actions.Add(new AdaptiveSubmitAction

            {

                Title = "Send greeting.",

                Data = "Hello little bot! How are you today?"

            });

            var adaptiveCardAtt = new Attachment()
            {

                ContentType = "application/vnd.microsoft.card.adaptive",

                Content = adaptiveCard

            };

            var message = MessageFactory.Attachment(adaptiveCardAtt);

            await turnContext.SendActivityAsync(message);
        }
    }
}
