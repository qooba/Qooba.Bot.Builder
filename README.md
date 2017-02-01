# Qooba.Bot.Builder
Library for creating chatbots 

## Example usage:

Example

```csharp
[BotAuthentication]
public class MessagesController : ApiController
{
    public async Task<HttpResponseMessage> Post([FromBody]Activity activity) => await new BotBuilder()
            .RegisterType<IWitModel, WitModel>()
            .RegisterType<ILogger, Logger>()
            .RegisterDefault<DefaultDialogSimpleResume>()
            .RegisterSimple<ImageSearchDialogSimpleResume>("imageSearch")
            .RegisterSimple<EmotionDialogSimpleResume>("emotion")
            .RegisterForm<HungryForm, HungryDialogResume>("hungry")
            .RegisterForm<VoteForm, VoteDialogResume>("vote")
            .RegisterSimple<VoteSimpleResume>("readVote")
            .RegisterForm<ShoppingForm, ShoppingDialogResume, ShoppingFormBuilder>("shopping")
            .RegisterFactory<NewWitDialogFactory>()
            .SendAsync(activity);
}

[Serializable]
public class WitModel : IWitModel
{
    public string ApiKey => "XXX";
}
```
