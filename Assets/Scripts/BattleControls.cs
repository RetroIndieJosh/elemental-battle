// GENERATED AUTOMATICALLY FROM 'Assets/ScriptableObjects/Battle Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class BattleControls : IInputActionCollection
{
    private InputActionAsset asset;
    public BattleControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Battle Controls"",
    ""maps"": [
        {
            ""name"": ""Top Menu"",
            ""id"": ""4ef48dd8-9003-4238-92ba-04613c49ae24"",
            ""actions"": [
                {
                    ""name"": ""Menu 1"",
                    ""id"": ""a751fb33-899a-4e88-929f-d514ff3ef236"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Menu 2"",
                    ""id"": ""ba48886f-a7c3-4d90-bdd8-900776c944cc"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Menu 3"",
                    ""id"": ""e3cf1760-0810-4bc1-9afc-ad06c4f309fd"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Menu 4"",
                    ""id"": ""295b33ce-8ced-4d39-bbcb-05be35b13252"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""Back"",
                    ""id"": ""b9a126ee-72d9-4add-8751-5a6aa20561aa"",
                    ""expectedControlLayout"": """",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f4ee841a-967c-42c4-831a-90e7dacdcf8e"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""66bd919b-8388-45f8-abc1-db39eb6165a2"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""5bf0e68e-31c8-447f-bf35-51ef1994540a"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""a9ed2f83-64b5-48b5-8b1a-5811a412d574"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""38ff66e7-dd20-4998-8049-12a843151705"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""16305bec-48b3-4aa0-a1cd-2a241cad457c"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""d25baf29-e841-4f78-8c03-21496f63b0d7"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""8701d14c-2568-444d-b71c-7859a873fabc"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""dd0825bf-990e-42b6-a667-ec3f3873942c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""c70c10ef-20aa-4ca6-ba95-841d72b8dead"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Top Menu
        m_TopMenu = asset.GetActionMap("Top Menu");
        m_TopMenu_Menu1 = m_TopMenu.GetAction("Menu 1");
        m_TopMenu_Menu2 = m_TopMenu.GetAction("Menu 2");
        m_TopMenu_Menu3 = m_TopMenu.GetAction("Menu 3");
        m_TopMenu_Menu4 = m_TopMenu.GetAction("Menu 4");
        m_TopMenu_Back = m_TopMenu.GetAction("Back");
    }

    ~BattleControls()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes
    {
        get => asset.controlSchemes;
    }

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Top Menu
    private InputActionMap m_TopMenu;
    private ITopMenuActions m_TopMenuActionsCallbackInterface;
    private InputAction m_TopMenu_Menu1;
    private InputAction m_TopMenu_Menu2;
    private InputAction m_TopMenu_Menu3;
    private InputAction m_TopMenu_Menu4;
    private InputAction m_TopMenu_Back;
    public struct TopMenuActions
    {
        private BattleControls m_Wrapper;
        public TopMenuActions(BattleControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menu1 { get { return m_Wrapper.m_TopMenu_Menu1; } }
        public InputAction @Menu2 { get { return m_Wrapper.m_TopMenu_Menu2; } }
        public InputAction @Menu3 { get { return m_Wrapper.m_TopMenu_Menu3; } }
        public InputAction @Menu4 { get { return m_Wrapper.m_TopMenu_Menu4; } }
        public InputAction @Back { get { return m_Wrapper.m_TopMenu_Back; } }
        public InputActionMap Get() { return m_Wrapper.m_TopMenu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(TopMenuActions set) { return set.Get(); }
        public void SetCallbacks(ITopMenuActions instance)
        {
            if (m_Wrapper.m_TopMenuActionsCallbackInterface != null)
            {
                Menu1.started -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu1;
                Menu1.performed -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu1;
                Menu1.canceled -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu1;
                Menu2.started -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu2;
                Menu2.performed -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu2;
                Menu2.canceled -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu2;
                Menu3.started -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu3;
                Menu3.performed -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu3;
                Menu3.canceled -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu3;
                Menu4.started -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu4;
                Menu4.performed -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu4;
                Menu4.canceled -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnMenu4;
                Back.started -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnBack;
                Back.performed -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnBack;
                Back.canceled -= m_Wrapper.m_TopMenuActionsCallbackInterface.OnBack;
            }
            m_Wrapper.m_TopMenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                Menu1.started += instance.OnMenu1;
                Menu1.performed += instance.OnMenu1;
                Menu1.canceled += instance.OnMenu1;
                Menu2.started += instance.OnMenu2;
                Menu2.performed += instance.OnMenu2;
                Menu2.canceled += instance.OnMenu2;
                Menu3.started += instance.OnMenu3;
                Menu3.performed += instance.OnMenu3;
                Menu3.canceled += instance.OnMenu3;
                Menu4.started += instance.OnMenu4;
                Menu4.performed += instance.OnMenu4;
                Menu4.canceled += instance.OnMenu4;
                Back.started += instance.OnBack;
                Back.performed += instance.OnBack;
                Back.canceled += instance.OnBack;
            }
        }
    }
    public TopMenuActions @TopMenu
    {
        get
        {
            return new TopMenuActions(this);
        }
    }
    public interface ITopMenuActions
    {
        void OnMenu1(InputAction.CallbackContext context);
        void OnMenu2(InputAction.CallbackContext context);
        void OnMenu3(InputAction.CallbackContext context);
        void OnMenu4(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
}
