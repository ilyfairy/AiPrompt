using System.Windows.Controls;
using Microsoft.Extensions.Logging;

namespace AiPrompt.Views.Components;

public class TestComponent : UserControl
{
    public TestComponent()
    {
        var logger = App.GetService<ILogger<TestComponent>>()!;
        logger.LogInformation("创建TestComponent实例");
    }
}
