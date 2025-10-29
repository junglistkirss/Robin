namespace Robin;

public enum TokenType
{
    Text,
    Variable,           // {{variable}}
    UnescapedVariable,  // {{{variable}}} or {{&variable}}
    SectionOpen,        // {{#section}}
    SectionClose,       // {{/section}}
    InvertedSection,    // {{^section}}
    Comment,            // {{! comment}}
    Partial             // {{> partial}}
}
