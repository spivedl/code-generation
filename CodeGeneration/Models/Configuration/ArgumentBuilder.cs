using System.Collections.Generic;

namespace CodeGeneration.Models.Configuration
{
    public class ArgumentBuilder
    {
        private ISet<string> _arguments;

        public ArgumentBuilder()
        {
            _arguments = new HashSet<string>();
        }

        public ArgumentBuilder(params string[] args)
        {
            _arguments = new HashSet<string>(args);
        }

        public ArgumentBuilder Add(string argument)
        {
            if (!_arguments.Contains(argument)) _arguments.Add(argument);

            return this;
        }

        public ArgumentBuilder Add(string argument, string value)
        {
            if (_arguments.Contains(argument)) return this;

            _arguments.Add(argument);
            _arguments.Add(value);

            return this;
        }

        public ArgumentBuilder Add(params string[] arguments)
        {
            foreach (var arg in arguments)
            {
                if (!_arguments.Contains(arg)) _arguments.Add(arg);
            }

            return this;
        }

        public ArgumentBuilder Remove(string argument)
        {
            if (argument.Contains(argument)) _arguments.Remove(argument);

            return this;
        }

        public ArgumentBuilder Clear()
        {
            _arguments.Clear();
            _arguments = new HashSet<string>();

            return this;
        }

        public bool Contains(string argument)
        {
            return _arguments.Contains(argument);
        }

        public string Build(string separator = " ")
        {
            return string.Join(separator, _arguments);
        }
    }
}
