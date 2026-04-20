namespace TransportApp.Mobile.Pages
{
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage(TaskDetailPageModel model)
        {
           
            BindingContext = model;
        }
    }
}