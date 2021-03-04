using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AwesomeApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        //Importamos Xamarin.Forms para poder habilitar la url en base al comando de Binding TapCommand definido el el MainPage.xaml
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public MainPage()
        {
            InitializeComponent();
            //This is optional, but provides better layout for the iPhone X 
            //hp On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            BindingContext = this;
        }

        /// <summary>
        /// Botón encargado de realizar en envio, previa invocación a la tarea de ValidateForm(), si la validación retorna verdadero (true) invocamos a la tarea SendEmail(...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void btnSend_Clicked(object sender, System.EventArgs e)
        {
            if (await ValidateForm()) {
                List<string> toAddress = new List<string>();
                toAddress.Add(txtTo.Text);
                await SendEmail(txtSubject.Text, txtBody.Text, toAddress);
            }
        }

        /// <summary>
        /// Validar las propiedades de la pantalla de envio de email
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ValidateForm()
        {
            //Valida si el valor en el Entry txtTo se encuentra vacio o es igual a Null
            if (String.IsNullOrWhiteSpace(txtTo.Text))
            {
                await this.DisplayAlert("Advertencia", "El campo tu-mail es obligatorio.", "OK");
                return false;
            }
            else {
                //Valida que el formato del correo sea valido
                bool isEmail = Regex.IsMatch(txtTo.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    await this.DisplayAlert("Advertencia", "El formato del correo electrónico es incorrecto, revíselo e intente nuevamente.", "OK");
                    return false;
                }
            }

            //Valida si el valor en el Entry txtSubject se encuentra vacio o es igual a Null
            if (String.IsNullOrWhiteSpace(txtSubject.Text))
            {
                await this.DisplayAlert("Advertencia", "El campo Asunto es obligatorio.", "OK");
                return false;
            }

            //Valida si el valor en el Entry se encuentra vacio o es igual a Null
            if (String.IsNullOrWhiteSpace(txtBody.Text))
            {
                var res = await DisplayAlert("Advertencia", "Aún no se a ingresado el mensaje, esta seguro de enviarlo", "OK", "Cancel");
                if (res)//True si se preciona OK, se esta confirmado que el correo se enviará sin mensaje
                {
                    return true;
                }
                else
                {//False si se preciona Cancel, y se debe ingresar el mensaje a enviar
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Tarea asincrona para realizar el envío del mensaje
        /// </summary>
        /// <param name="subject">Asunto del email</param>
        /// <param name="body">Cuerpo del mensaje del email</param>
        /// <param name="recipients">Destinatarios</param>
        /// <returns></returns>
        private async Task SendEmail(string subject, string body, List<string> recipients) 
        {
            try
            {
                //Propiedades del mensaje
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };
                //var fn = "Attachment.txt";
                //var file = Path.Combine(FileSystem.CacheDirectory, fn);
                //message.Attachments.Add(new EmailAttachment(file));

                //API que se encarga de abrir el cliente como el Gmail, Outlook u otros para realizar el envío del mensaje
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Email is not supported on this device 
                await DisplayAlert("Error", fnsEx.ToString(), "OK");
            }
            catch (Exception ex)
            {
                // Some other exception occurred
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            count++;
            ((Button)sender).Text = $"You clicked {count} times.";
        }
    }
}
