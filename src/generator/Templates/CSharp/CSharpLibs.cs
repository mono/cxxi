﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Templates {
    using System;
    
    
    public partial class CSharpLibs : LibsBase {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 2 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write("// -------------------------------------------------------------------------\n//  " +
                    "C++ library declarations\n//  Generated on ");
            
            #line default
            #line hidden
            
            #line 4 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( DateTime.Now ));
            
            #line default
            #line hidden
            
            #line 4 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write("\n//\n//  This file was auto generated. Do not edit.\n// ---------------------------" +
                    "----------------------------------------------\n\nusing System;\nusing Mono.Cxxi;\nu" +
                    "sing Mono.Cxxi.Abi;\n\nnamespace ");
            
            #line default
            #line hidden
            
            #line 13 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( Generator.Lib.BaseNamespace ));
            
            #line default
            #line hidden
            
            #line 13 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(" {\n\n\tpublic static partial class Libs {\n");
            
            #line default
            #line hidden
            
            #line 16 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
 foreach (var lib in Libs) { 
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write("\t\tpublic static readonly CppLibrary ");
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( CSharpLanguage.SafeIdentifier (lib.BaseName) ));
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(" = new CppLibrary (\"");
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( lib.BaseName ));
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( lib.AbiType ));
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(".Instance, InlineMethods.");
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( lib.InlinePolicy ));
            
            #line default
            #line hidden
            
            #line 17 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write(");\n");
            
            #line default
            #line hidden
            
            #line 18 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
 } 
            
            #line default
            #line hidden
            
            #line 19 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpLibs.tt"
            this.Write("\t}\n}\n");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        protected override void Initialize() {
            base.Initialize();
        }
    }
}
