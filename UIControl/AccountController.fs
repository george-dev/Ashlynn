namespace Ashlynn.UI

open System
open System.Web
open System.Web.Mvc
open System.Web.Security
open System.ComponentModel.DataAnnotations

type UserModel() = 
    [<Required>]
    [<Display(Name = "User Name")>]
    member val Name = "" with get, set

    [<Required>]
    [<Display(Name = "Password")>]
    [<DataType(DataType.Password)>]
    member val Password = "" with get, set

    [<Display(Name = "Remember me")>]
    member val RememberMe = false with get, set

[<Authorize>]
[<HandleError>]
type AccountController() = 
    inherit Controller()

    [<AllowAnonymous>]
    member x.LogOn() = 
        x.View() :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member x.LogOn(model: UserModel) = 
        if model.Name = UIConfig.User && model.Password = UIConfig.Password then
            FormsAuthentication.SetAuthCookie(model.Name, model.RememberMe)
            base.RedirectToAction("Index", "Home") :> ActionResult
        else
            base.ModelState.AddModelError("", "User or Password is incorrect.");
            base.View(model) :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    member x.LogOff() = 
        FormsAuthentication.SignOut()
        base.RedirectToAction("Index") :> ActionResult
        

