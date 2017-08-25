using Android.App;

namespace Employees.Droid
{
    public class DialogService
    {
        public void ShowMessage(Activity activity, string title, string message)
        {
            var builder = new AlertDialog.Builder(activity);
            var alert = builder.Create();
            alert.SetTitle(title);
            alert.SetIcon(Resource.Drawable.Icon);
            alert.SetMessage(message);
            alert.SetButton("Aceptar", (s, ev) => { });
            alert.Show();
        }
    }
}