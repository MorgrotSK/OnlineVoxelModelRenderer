using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FE3.Pages.WorldPages
{
    public partial class WorldCreatorPage : ComponentBase
    {
        [Inject]
        private NavigationManager Nav { get; set; } = default!;

        protected string WorldId { get; set; } = string.Empty;

        protected bool IsGenerateDisabled =>
            string.IsNullOrWhiteSpace(WorldId);

        protected void Generate()
        {
            if (IsGenerateDisabled)
                return;

            Nav.NavigateTo($"/world/{Uri.EscapeDataString(WorldId)}");
        }

        protected void HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
                Generate();
        }
    }
}