using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc.Facebook;
using Microsoft.AspNet.Mvc.Facebook.Client;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        #region [ Main ]

        private Facebook.FacebookClient FBClient(string accessToken)
        {
            return new Facebook.FacebookClient(accessToken)
            {
                AppId = Microsoft.AspNet.Mvc.Facebook.GlobalFacebookConfiguration.Configuration.AppId,
                AppSecret = Microsoft.AspNet.Mvc.Facebook.GlobalFacebookConfiguration.Configuration.AppSecret
            };
        }

        ///<summary>
        ///Controla el flujo de la aplicacion segun el estado del usuario
        ///</summary>
        ///<param name="user">Obtener los datos del usuario de la base de datos.</param>
        ///<param name="actionName">Nombre de la accion que dispara el evento.</param>
        private string AppFacebookControl(ref User user, string actionName)
        {
            string response = null; //Mostrar la vista del controlador actual por defecto
            string accessToken = (string)Session["AccessToken"];
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                try
                {
                    dynamic me = FBClient(accessToken).Get("me"); //Obtener datos de un Usuario desde Facebook.
                    string id = me != null ? me.id : null;//Obtener Id de Facebook, de lo contrario retorna null.
                    if (!string.IsNullOrWhiteSpace(id)) //Comprobar el id del usuario.
                    {
                        using (var context = new Models.DataContext())
                        {
                            user = context.Users.SingleOrDefault(x => x.IdUser == id); //Obtener al Usuario de la Base de Datos.
                        }
                        if (user != null)
                        {
                            if (actionName != "Ok")
                            {
                                response = "Index"; //El usuario instalo la aplicacion y realizo el registro exitosamente.
                            }
                        }
                        else
                        {
                            if (actionName != "Register")
                            {
                                response = "Register"; //El usuario instalo la aplicacion pero no ha registrado sus datos.
                            }
                        }
                        return response;
                    }
                }
                catch
                {
                    Session.Remove("AccessToken"); //El token ha expirado
                }
            }
            if (actionName != "Install")
            {
                response = "Install"; //El usuario no ha instalado la aplicacion o el token ha expirado.
            }
            return response;
        }

        ///<summary>
        ///El usuario debe instalar la aplicacion, se encarga de controlar si el usuario esta logueado.
        ///</summary>
        public ActionResult Install(string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                Session["AccessToken"] = accessToken;
            }
            User user = null;
            string action = AppFacebookControl(ref user, "Install");
            if (action != null)
            {
                return RedirectToAction(action);
            }
            return View();
        }

        ///<summary>
        ///El usuario debe registrar sus datos en la base de datos.
        ///</summary>
        public ActionResult Register()
        {
            User user = null;
            string action = AppFacebookControl(ref user, "Register");
            if (action != null)
            {
                return RedirectToAction(action);
            }
            return View();
        }

        ///<summary>
        ///Es el Inicio de la aplicacion, despues de que el usuario instalo la aplicacion y registro sus datos.
        ///</summary>
        public ActionResult Index()
        {
            User user = null;
            string action = AppFacebookControl(ref user, "Ok");
            if (action != null)
            {
                return RedirectToAction(action);
            }
            return View();
        }

        ///<summary>
        ///Registrar los datos de un usuario en la base de datos.
        ///</summary>
        ///<param name="user">Datos ingresados por el usuario.</param>
        [HttpPost]
        public ActionResult NewUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", user);
            }
            else
            {
                //Registrar el Usuario en la Base de Datos si previamente no existe
                using (var context = new Models.DataContext())
                {
                    if (!context.Users.Any(x => x.IdUser == user.IdUser))
                    {
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region [ Other Methods ]

        #endregion

        #region [ Other Pages ]

        #endregion
    }
}
