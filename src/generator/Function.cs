using System;
using System.Collections.Generic;
using Mono.Cxxi;

public class Function : Namespace
{
    public Function(Node node) 
        : base(node)
    {
        Parameters = new List<Parameter> ();
    }
    
    public Access Access
    {
        get;
        set;
    }

    public CppType ReturnType
    {
        get;
        set;
    }

    public List<Parameter> Parameters
    {
        get;
        set;
    }

    // The C# method name
    public string FormattedName
    {
        get
        {
            return "" + Char.ToUpper (Name[0]) + Name.Substring (1);
        }
    }
    
    private string[] fullyQualifiedName;
    public override string[] FullyQualifiedName
    {
        get
        {
            if (fullyQualifiedName == null) {

                if (ParentNamespace == null) {
                    fullyQualifiedName = new string[] { Name };
                } else {
                    var parentFqn = ParentNamespace.FullyQualifiedName;
                    fullyQualifiedName = new string[parentFqn.Length + 1];
                    Array.Copy (parentFqn, fullyQualifiedName, parentFqn.Length);
                    fullyQualifiedName[parentFqn.Length] = this.FormattedName;
                }
            }
            return fullyQualifiedName;
        }
    }
}