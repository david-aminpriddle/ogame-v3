using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace OGame.MVC.Areas.Game.Services;

using Controllers;

public static class NavigationCache
{
    private static List<ControllerInfo>? s_ControllersWithActions;
    private static readonly object LockObject = new();

    public record ActionInfo(string Name);
    public record ControllerInfo(string Name, List<ActionInfo> Actions);

    private static readonly Type[] Controllers =
    {
        typeof(HomeController),
        typeof(ColoniesController),
        typeof(ProductionsController),
        typeof(ShipsController),
        typeof(ResourcesController),
        typeof(ResearchController),
        typeof(QuestsController),
    };

    public static List<ControllerInfo> GetControllersWithActions()
    {
        if (s_ControllersWithActions is null)
        {
            LoadControllersWithActions();
        }

        return s_ControllersWithActions!;
    }

    private static void LoadControllersWithActions()
    {
        lock (LockObject)
        {
            var actionResultInterfaceType = typeof(IActionResult);
            s_ControllersWithActions ??= Controllers
                .Select(type => new ControllerInfo(
                    type.Name.Replace("Controller", string.Empty),
                    type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(method => method.IsPublic
                                         && method.Name != "Index" // we'll handle this through the controller
                                         && !method.IsDefined(typeof(NonActionAttribute), true)
                                         && method.ReturnType.IsAssignableFrom(actionResultInterfaceType)
                                         && !method.IsSpecialName)
                        .Select(method => new ActionInfo(method.Name))
                        .ToList()
                ))
                .ToList();
        }
    }
}
