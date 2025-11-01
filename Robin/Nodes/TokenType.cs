namespace Robin.Nodes;

public enum TokenType
{
    Text,
    Variable,           // {{variable}}
    UnescapedVariable,  // {{{variable}}} or {{&variable}}
    SectionOpen,        // {{#section}}
    InvertedSection,    // {{^section}}
    SectionClose,       // {{/section}}
    Comment,            // {{! comment}}
    PartialDefine,      // {{< partial}}
    PartialCall,        // {{> partial}}
}
