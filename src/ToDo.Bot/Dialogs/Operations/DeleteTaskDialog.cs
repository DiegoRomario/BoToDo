using Microsoft.Bot.Builder.Dialogs;

namespace ToDo.Bot.Dialogs.Operations
{
    public class DeleteTaskDialog : ComponentDialog
    {
        public DeleteTaskDialog() : base(nameof(DeleteTaskDialog))
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
