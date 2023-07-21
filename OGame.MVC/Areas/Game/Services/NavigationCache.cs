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
            s_ControllersWithActions ??= typeof(HomeController).Assembly.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .Where(type => type.Namespace.Contains("Game.Controllers"))
                .Select(type => new ControllerInfo(
                    type.Name.Replace("Controller", string.Empty),
                    type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                        .Where(method => method.IsPublic
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
