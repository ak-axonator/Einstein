using KdlDotNet;

namespace WebMagic
{
    public class Section
    {
        public KDLString HandlerName { get; internal set; }
        public KDLString HandlerClassName { get; internal set; }
        public KDLBoolean isAbstract { get; internal set; }
        public string Identifier { get; private set; }
        internal ISectionHandler Handler { get; private set; }
        internal List<Instruction> DefinedInstructions { get; }
        public Section(string identifier, KDLString handlerClassName, KDLString handlerName, KDLBoolean isAbstract)
        {
            this.Identifier = identifier;
            this.HandlerClassName = handlerClassName;
            this.HandlerName = handlerName;
            this.isAbstract = isAbstract;
            this.Handler = HandlerFactory.GetHandler(this.HandlerClassName.Value);
            this.DefinedInstructions = new List<Instruction>();
        }

        internal List<string> Validate(KDLNode node)
        {
            List<string> errors = new List<string>();
            foreach (KDLNode n in node.Child.Nodes)
            {
                Instruction? instruction = DefinedInstructions.Find(instruction => instruction.Identifier == n.Identifier);
                if (instruction == null)
                {
                    errors.Add($"Error: Instruction {n.Identifier} is not defined.");
                }
                else
                {
                    //switch (instruction)
                }
            }
            return errors;
        }
    }
}