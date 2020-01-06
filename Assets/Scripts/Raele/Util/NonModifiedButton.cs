using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

// Use InputBindingComposite<TValue> as a base class for a composite that returns
// values of type TValue.
// NOTE: It is possible to define a composite that returns different kinds of values
//       but doing so requires deriving directly from InputBindingComposite.
#if UNITY_EDITOR
[InitializeOnLoad] // Automatically register in editor.
#endif
public class NonModifiedButton : InputBindingComposite<float>
{
    // Each part binding is represented as a field of type int and annotated with
    // InputControlAttribute. Setting "layout" allows to restrict the controls that
    // are made available for picking in the UI.
    //
    // On creation, the int value will be set to an integer identifier for the binding
    // part. This identifier can be used to read values from InputBindingCompositeContext.
    // See ReadValue() below.
    [InputControl(layout = "Button")] public int button;

    [InputControl(layout = "Button")] public int modifier0;
    [InputControl(layout = "Button")] public int modifier1;
    [InputControl(layout = "Button")] public int modifier2;
    [InputControl(layout = "Button")] public int modifier3;
    [InputControl(layout = "Button")] public int modifier4;
    [InputControl(layout = "Button")] public int modifier5;
    [InputControl(layout = "Button")] public int modifier6;
    [InputControl(layout = "Button")] public int modifier7;

    // Any public field that is not annotated with InputControlAttribute is considered
    // a parameter of the composite. This can be set graphically in the UI and also
    // in the data (e.g. "custom(floatParameter=2.0)").
    // public float floatParameter;
    // public bool boolParameter;

    // This method computes the resulting input value of the composite based
    // on the input from its part bindings.
    public override float ReadValue(ref InputBindingCompositeContext context)
        => (this.modifier0 == default || !context.ReadValueAsButton(this.modifier0))
            && (this.modifier1 == default || !context.ReadValueAsButton(this.modifier1))
            && (this.modifier2 == default || !context.ReadValueAsButton(this.modifier2))
            && (this.modifier3 == default || !context.ReadValueAsButton(this.modifier3))
            && (this.modifier4 == default || !context.ReadValueAsButton(this.modifier4))
            && (this.modifier5 == default || !context.ReadValueAsButton(this.modifier5))
            && (this.modifier6 == default || !context.ReadValueAsButton(this.modifier6))
            && (this.modifier7 == default || !context.ReadValueAsButton(this.modifier7))
            ? context.ReadValue<float>(this.button)
            : default;

    // This method computes the current actuation of the binding as a whole.
    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        => this.ReadValue(ref context);

    static NonModifiedButton()
    {
        // Can give custom name or use default (type name with "Composite" clipped off).
        // Same composite can be registered multiple times with different names to introduce
        // aliases.
        //
        // NOTE: Registering from the static constructor using InitializeOnLoad and
        //       RuntimeInitializeOnLoadMethod is only one way. You can register the
        //       composite from wherever it works best for you. Note, however, that
        //       the registration has to take place before the composite is first used
        //       in a binding. Also, for the composite to show in the editor, it has
        //       to be registered from code that runs in edit mode.
        InputSystem.RegisterBindingComposite<NonModifiedButton>();
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init() {} // Trigger static constructor.
}