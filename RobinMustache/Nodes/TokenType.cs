namespace RobinMustache.Nodes;

public enum TokenType
{
    Text,
    Whitepsaces,
    LineBreak,
    Variable,           // {{variable}}
    UnescapedVariable,  // {{{variable}}} or {{&variable}}
    SectionOpen,        // {{#section}}
    InvertedSection,    // {{^section}}
    SectionClose,       // {{/section}}
    Comment,            // {{! comment}}
    PartialDefine,      // {{< partial}}
    PartialCall,        // {{> partial}}
    EOF,
}
