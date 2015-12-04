using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace CSLibTodoTxt
{
    public class Todo
    {
        public enum Priorities { A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, UNSET }
        public static Regex DatePattern = new Regex(@"^(\d{4}-\d{2}-\d{2})$");
        public static string GetNow()
        {
            DateTime date = DateTime.Now;
            return date.Year.ToString().PadLeft(4, '0') + "-" + date.Month.ToString().PadLeft(2, '0') + "-" + date.Day.ToString().PadLeft(2, '0');
        }

        private List<TodoItem> items;
        public List<TodoItem> Items
        {
            get
            {
                return this.items;
            }
        }

        public Todo()
        {
            this.items = new List<TodoItem>();
        }
    }

    public class TodoItem
    {
        private string raw;
        private bool completed;
        public bool Completed
        {
            get
            {
                return completed;
            }
            set
            {
                completed = value;
                if (value == true && Priority != Todo.Priorities.UNSET)
                {
                    custom["pri"] = Priority.ToString();
                    Priority = Todo.Priorities.UNSET;
                    DateCompleted = Todo.GetNow();
                }
            }
        }

        public string DateAdded
        {
            get;
            set;
        }

        public string DateCompleted
        {
            get;
            set;
        }

        private Todo.Priorities priority;
        public Todo.Priorities Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }
        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
        }
        private List<String> contexts;
        public List<String> Contexts
        {
            get
            {
                return this.contexts;
            }
        }
        private List<String> projects;
        public List<String> Projects
        {
            get
            {
                return this.projects;
            }
        }
        private Dictionary<String, String> custom;
        public Dictionary<string, string> Custom
        {
            get
            {
                return custom;
            }
        }
        
        public string GetCustom(String key)
        {
            try
            {
                return String.Format("{0}:{1}", key, custom[key]);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void SetCustom(string key, string value)
        {
            custom[key] = value;
        }

        public int TextStart;

        public TodoItem(string input)
        {
            this.raw = input;
            this.Completed = false;
            this.DateAdded = "";
            this.DateCompleted = "";
            this.priority = Todo.Priorities.UNSET;
            this.text = "";
            this.contexts = new List<String>();
            this.projects = new List<String>();
            this.custom = new Dictionary<String, String>();
            this.TextStart = 0;
            parse();
        }

        private void parse()
        {
            if(this.raw != null)
            {
                string[] words = this.raw.Split(' ');
                int textEnd = words.Length;

                // Get completed
                if (words[0].Length == 1 && words[0] == "x")
                {
                    this.Completed = true;
                    TextStart += 1;
                }

                // Get completion date
                if (TextStart == 1)
                {
                    // completed is set
                    Match isDate = Todo.DatePattern.Match(words[TextStart]);
                    if (isDate.Success)
                    {
                        this.DateCompleted = words[TextStart];
                        TextStart += 1;
                    }
                }

                // Get priority
                if (words[TextStart].StartsWith("(") && words[TextStart].EndsWith(")") && words[TextStart].Length == 3 && char.IsUpper(words[TextStart][1]))
                {
                    this.priority = (Todo.Priorities) Enum.Parse(typeof(Todo.Priorities), words[TextStart][1].ToString());
                    TextStart += 1;
                }
                else
                {
                    this.priority = Todo.Priorities.UNSET;
                }

                // Get added date
                {
                    // added is set
                    Match isDate = Todo.DatePattern.Match(words[TextStart]);
                    if (isDate.Success)
                    {
                        this.DateAdded = words[TextStart];
                        TextStart += 1;
                    }
                }

                // Set text...
                this.text = this.raw;

                // Get contexts, projects and custom
                for (int i = TextStart; i < textEnd; i++)
                {
                    if (words[i].StartsWith("@"))
                    {
                        if (!this.contexts.Contains(words[i]))
                        {
                            this.contexts.Add(words[i]);
                        }
                    }
                    if (words[i].StartsWith("+"))
                    {
                        if (!this.projects.Contains(words[i]))
                        {
                            this.projects.Add(words[i]);
                        }
                    }
                    if (words[i].Contains(":"))
                    {
                        string[] c = words[i].Split(':');
                        if (c.Length == 2)
                        {
                            SetCustom(c[0], c[1]);
                        }
                    }
                }
            }
        }
    }

    public class Serializer
    {
        public static String serialize(TodoItem item)
        {
            List<string> output = new List<string>();
            if (item.Completed)
            {
                output.Add("x");
                if (item.DateCompleted != "")
                {
                    output.Add(item.DateCompleted);
                }
            }
            if (item.Priority != Todo.Priorities.UNSET)
            {
                output.Add("(" + item.Priority.ToString() + ")");
            }
            if (item.DateAdded != "")
            {
                output.Add(item.DateAdded);
            }
            output.AddRange(item.Text.Split(' ').Skip(item.TextStart));
            foreach (KeyValuePair<string, string> entry in item.Custom)
            {
                output.Add(String.Format("{0}:{1}", entry.Key, entry.Value));
            }
            return String.Join(" ", output);
        }

        public static String serialize(Todo todo)
        {
            var output = "";
            foreach (var item in todo.Items)
            {
                output += serialize(item) + Environment.NewLine;
            }
            return output;
        }
    }
}
