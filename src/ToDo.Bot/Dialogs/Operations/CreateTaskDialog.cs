using Microsoft.Bot.Builder.Dialogs;
namespace ToDo.Bot.Dialogs.Operations
{
    public class CreateTaskDialog : ComponentDialog
    {
        public CreateTaskDialog() : base(nameof(CreateTaskDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

    }
}
