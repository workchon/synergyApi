using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace IsoApiSynergy
{
    public class clsMail
    {
        /// <summary>
        /// Envía correo electrónico con el mensaje y asunto especificados al destinatario y copias que se le indique
        /// </summary>
        /// <param name="Remitente">Remitente al que se le enviará el correo</param>
        /// <param name="NombreR">Nombre del remitente</param>
        /// <param name="Destino">Destino a donde será enviado el correo</param>
        /// <param name="Asunto">Asunto que llevará el correo</param>
        /// <param name="Mensaje">Mensaje que llevará el correo</param>
        /// <param name="copiaPara">Copia a quien/quienes irá dirigido el correo</param>
        /// <returns></returns>
        public Boolean SMTPMail(string Remitente, string NombreR, string PasswordR, List<string> Destino, string Asunto, string Mensaje)
        {
            using (MailMessage Mail = new MailMessage())
            {
                try
                {
                    //Se agrega el remitente
                    MailAddress remitenteInfo = new MailAddress(Remitente, NombreR);
                    Mail.From = remitenteInfo;

                    //Se agrega el destino                    
                    foreach (string destino in Destino)
                    {
                        Mail.To.Add(destino);
                    }
                    //Mail.To.Add("alan.miranda@synergysft.com");

                    //Se agrega el asunto
                    Mail.Subject = Asunto;

                    //Se agregan las copias
                    /*if (copiaPara != null)
                    {
                        foreach (string cc in copiaPara)
                        {
                            Mail.CC.Add(cc);
                        }
                    }*/
                    //Mail.CC.Add("Mirandaalan.miranda73@gmail.com");

                    //¿========================BUSCAR LAS IMAGENES DENTRO DEL HTML PARA INTEGRARLAS AL CUERPO DEL CORREO==================                    
                    int index = 0, inicioBus = 0, finBus = 0;
                    List<string> listaImg = new List<string>();//Almacena la cadena base64 de las imagenes                
                    List<string> listaBase = new List<string>();//Almacena la cadena base64 junto con el src

                    int inicioEx = 0, finEx = 0;//Variables para identificar la posición de la extencion de imagen en la cadena
                    List<string> ext = new List<string>();//Lista de las extenciones de las imagenes agregadas

                    while ((finBus + 5) < Mensaje.Length)//DUDA CON LA CONDICIÓN!!!!!!!!!!!
                    {
                        inicioBus = Mensaje.IndexOf("<img", index);//busca el index de '<img'

                        if (inicioBus < 0)
                            break;

                        finBus = Mensaje.IndexOf("/>", inicioBus);//Busca el final del tag <img>

                        if (finBus < 0)
                            break;

                        index = finBus;
                        string Cad = Mensaje.Substring(inicioBus, (finBus - inicioBus) + 2);//obtiene tódo el tag <img>

                        //Obtener la extención de la imagen
                        inicioEx = Cad.IndexOf("/") + 1;
                        finEx = (Cad.IndexOf(";") - Cad.IndexOf("/")) - 1;
                        ext.Add(Cad.Substring(inicioEx, finEx));

                        //Reemplazar cadenas
                        Cad = Cad.Replace("<img src=\"data:image/" + ext[ext.Count - 1] + ";base64,", "");//quita la parte principal que no pertenezca al base64
                        Cad = Cad.Substring(0, Cad.IndexOf("\""));//Extrae substring partiendo de la posición 0 hasta la posición de la (")

                        listaBase.Add("src=\"data:image/" + ext[ext.Count - 1] + ";base64," + Cad + "\"");//Agrega la cadena base64 junto con el src a la listaBase

                        listaImg.Add(Cad);//Agrega sólo la cadena base64 a la listaImg
                    }

                    List<string> listaNewSrc = new List<String>();//Almacena el nuevo src de las imagenes
                    LinkedResource headerImage;
                    Byte[] bitmapData;
                    System.IO.MemoryStream streamBitmap;
                    List<LinkedResource> listaLiked = new List<LinkedResource>();//almacena los LinkedResource

                    if (listaImg != null)
                    {
                        for (int i = 0; i < listaImg.Count; i++)
                        {
                            bitmapData = Convert.FromBase64String(FixBase64ForImage(listaImg[i]));//crea un bitmap de la imagen
                            streamBitmap = new System.IO.MemoryStream(bitmapData);//crea un stream del bitmap

                            headerImage = new LinkedResource(streamBitmap, System.Net.Mime.MediaTypeNames.Image.Jpeg);//crea un headerImage del stream
                            headerImage.ContentId = "imagen" + i.ToString();//agrega un contentId
                            listaNewSrc.Add("src=\"cid:imagen" + i.ToString() + "\"");//agrega el src nuevo
                            headerImage.ContentType = new ContentType("image/" + ext[i]);
                            listaLiked.Add(headerImage);//agrega el LinkedResource a la lista
                        }
                    }

                    if (listaImg != null)
                    {
                        for (int j = 0; j < listaImg.Count; j++)
                        {
                            Mensaje = Mensaje.Replace(listaBase[j], listaNewSrc[j]);//Reemplaza las cadenas base64 por el nuevo src
                        }
                    }

                    //Crea AlternateView
                    AlternateView av = AlternateView.CreateAlternateViewFromString(Mensaje, null, System.Net.Mime.MediaTypeNames.Text.Html);//crea un alternateview            

                    if (listaLiked != null)
                    {
                        for (int i = 0; i < listaLiked.Count; i++)
                        {
                            av.LinkedResources.Add(listaLiked[i]);//añade cada LinkedResource
                        }
                    }
                    //===================================================================================================================

                    //Agrega el AlternateView creado
                    if (av != null)
                        Mail.AlternateViews.Add(av);

                    //Se agrega el nuevo mensaje con las nuevas rutas de las imagenes (si es que se añadieron imagenes)
                    Mail.Body = Mensaje;
                    Mail.Headers.Add("Disposition-Notification-To", Remitente); //Notifica al remitente cuando el correo es leido
                    Mail.IsBodyHtml = true;

                    //ADJUNTA EL PDF
                    /*if (pdf != null)
                    {
                        if (nombreArchivo != "")
                            Mail.Attachments.Add(new Attachment(pdf, nombreArchivo + ".pdf", MediaTypeNames.Application.Pdf));
                        else if (nombreArchivo == "")
                            Mail.Attachments.Add(new Attachment(pdf, "Documento.pdf", MediaTypeNames.Application.Pdf));
                    }*/

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.mapco.com.mx";
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential(Remitente, PasswordR);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;

                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    smtp.Send(Mail);

                    return true;
                }
                catch (SmtpException ex)
                {
                    string error = ex.Message;
                    return false;
                }
            }
        }

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }


        public bool SMTPMail_recuperacion(string pAsunto, string pCuerpo, List<string> Destinatario, string CorreoRemitente, string NombreRemitente)
        {
            string mensajeError = "";
            try
            {
                using (MailMessage Mail = new MailMessage())
                {
                    //Se agrega el remitente
                    MailAddress remitenteInfo = new MailAddress(CorreoRemitente, NombreRemitente);
                    Mail.From = remitenteInfo;
                    //Se agrega el destino                    
                    foreach (string destino in Destinatario)
                    {
                        Mail.To.Add(destino);
                    }
                    //Se agrega el asunto
                    Mail.Subject = pAsunto;

                    //Buscar los <img> y usar el siguiente método
                    int index = 0, inicioBus = 0, finBus = 0;
                    List<string> listaImg = new List<string>();//Almacena la cadena base64 de las imagenes                
                    List<string> listaBase = new List<string>();//Almacena la cadena base64 junto con el src

                    int inicioEx = 0, finEx = 0;//Variables para identificar la posición de la extencion de imagen en la cadena
                    List<string> ext = new List<string>();//Lista de las extenciones de las imagenes agregadas

                    while ((finBus + 5) < pCuerpo.Length)//DUDA CON LA CONDICIÓN!!!!!!!!!!!
                    {
                        inicioBus = pCuerpo.IndexOf("<img", index);//busca el index de '<img'

                        if (inicioBus < 0)
                            break;

                        finBus = pCuerpo.IndexOf(">", inicioBus);//Busca el final del tag <img>

                        if (finBus < 0)
                            break;

                        index = finBus;
                        string Cad = pCuerpo.Substring(inicioBus, (finBus - inicioBus) + 2);//obtiene tódo el tag <img>

                        //Obtener la extención de la imagen
                        inicioEx = Cad.IndexOf("/") + 1;
                        finEx = (Cad.IndexOf(";") - Cad.IndexOf("/")) - 1;
                        ext.Add(Cad.Substring(inicioEx, finEx));

                        //Reemplazar cadenas
                        Cad = Cad.Replace("<img src=\"data:image/" + ext[ext.Count - 1] + ";base64,", "");//quita la parte principal que no pertenezca al base64
                        Cad = Cad.Substring(0, Cad.IndexOf("\""));//Extrae substring partiendo de la posición 0 hasta la posición de la (")

                        listaBase.Add("src=\"data:image/" + ext[ext.Count - 1] + ";base64," + Cad + "\"");//Agrega la cadena base64 junto con el src a la listaBase

                        listaImg.Add(Cad);//Agrega sólo la cadena base64 a la listaImg
                    }

                    List<string> listaNewSrc = new List<string>();//Almacena el nuevo src de las imagenes
                    LinkedResource headerImage;
                    Byte[] bitmapData;
                    System.IO.MemoryStream streamBitmap;
                    List<LinkedResource> listaLiked = new List<LinkedResource>();//almacena los LinkedResource
                    for (int i = 0; i < listaImg.Count; i++)
                    {
                        bitmapData = Convert.FromBase64String(FixBase64ForImage(listaImg[i]));//crea un bitmap de la imagen
                        streamBitmap = new System.IO.MemoryStream(bitmapData);//crea un stream del bitmap

                        headerImage = new LinkedResource(streamBitmap, System.Net.Mime.MediaTypeNames.Image.Jpeg);//crea un headerImage del stream
                        headerImage.ContentId = "companyLogo" + i.ToString();//agrega un contentId
                        listaNewSrc.Add("src=\"cid:companyLogo" + i.ToString() + "\"");//agrega el src nuevo
                        headerImage.ContentType = new ContentType("image/" + ext[i]);
                        listaLiked.Add(headerImage);//agrega el LinkedResource a la lista
                    }

                    for (int j = 0; j < listaImg.Count; j++)
                    {
                        pCuerpo = pCuerpo.Replace(listaBase[j], listaNewSrc[j]);//Reemplaza las cadenas base64 por el nuevo src
                    }

                    //Crea AlternateView
                    AlternateView av = AlternateView.CreateAlternateViewFromString(pCuerpo, null, System.Net.Mime.MediaTypeNames.Text.Html);//crea un alternateview            
                    for (int i = 0; i < listaLiked.Count; i++)
                    {
                        av.LinkedResources.Add(listaLiked[i]);//añade cada LinkedResource
                    }

                    //Agrega el asunto y el body para posteriormente enviar el mail
                    Mail.Subject = pAsunto;
                    Mail.Body = pCuerpo;
                    Mail.Headers.Add("Disposition-Notification-To", "noreply@synergysft.com"); //Notifica al remitente cuando el correo es leido
                    Mail.AlternateViews.Add(av);//Agrega el AlternateView creado

                    Mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();

                    //smtp.Host = "smtp.live.com";
                    smtp.Host = "mail.synergysft.com";
                    smtp.Port = 587;
                  //  smtp.Port = 30;
                    smtp.UseDefaultCredentials = true;

                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential("noreply@synergysft.com", "S0p0rt3.Syn!!");
                    smtp.EnableSsl = true;
                    smtp.Credentials = NetworkCred;
                    //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                    //smtp.Port = 587;
                    smtp.Send(Mail);
                    return true;
                }
            }
            catch (Exception err)
            {
                return false;
                throw err;
            }
        }
    }
}
