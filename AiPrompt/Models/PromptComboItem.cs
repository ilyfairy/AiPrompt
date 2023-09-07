using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiPrompt.Models
{
    public class PromptComboItem : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string[] Prompts { get; set; } = Array.Empty<string>();
    }
}
