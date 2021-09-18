using Microsoft.Bot.Builder.Dialogs;

namespace ToDo.Bot.Dialogs.Operations
{
    public class ViewTaskDialog : ComponentDialog
    {
        public ViewTaskDialog() : base(nameof(ViewTaskDialog))
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
