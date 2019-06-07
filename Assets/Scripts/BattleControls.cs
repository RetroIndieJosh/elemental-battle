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
            ""name"": ""Menus"",
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
                },
                {
                    ""name"": ""Help"",
                    ""id"": ""4eda6b46-315a-4f28-93e2-133b3007aa00"",
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
                    ""path"": ""<Keyboard>/escape"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""f794bb11-9c8e-47d8-a5b9-0f144f628647"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Help"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""4f9408d4-d3ff-41c9-aa3b-ca24db3ee2c8"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Help"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Menus
        m_Menus = asset.GetActionMap("Menus");
        m_Menus_Menu1 = m_Menus.GetAction("Menu 1");
        m_Menus_Menu2 = m_Menus.GetAction("Menu 2");
        m_Menus_Menu3 = m_Menus.GetAction("Menu 3");
        m_Menus_Menu4 = m_Menus.GetAction("Menu 4");
        m_Menus_Back = m_Menus.GetAction("Back");
        m_Menus_Help = m_Menus.GetAction("Help");
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

    // Menus
    private InputActionMap m_Menus;
    private IMenusActions m_MenusActionsCallbackInterface;
    private InputAction m_Menus_Menu1;
    private InputAction m_Menus_Menu2;
    private InputAction m_Menus_Menu3;
    private InputAction m_Menus_Menu4;
    private InputAction m_Menus_Back;
    private InputAction m_Menus_Help;
    public struct MenusActions
    {
        private BattleControls m_Wrapper;
        public MenusActions(BattleControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menu1 { get { return m_Wrapper.m_Menus_Menu1; } }
        public InputAction @Menu2 { get { return m_Wrapper.m_Menus_Menu2; } }
        public InputAction @Menu3 { get { return m_Wrapper.m_Menus_Menu3; } }
        public InputAction @Menu4 { get { return m_Wrapper.m_Menus_Menu4; } }
        public InputAction @Back { get { return m_Wrapper.m_Menus_Back; } }
        public InputAction @Help { get { return m_Wrapper.m_Menus_Help; } }
        public InputActionMap Get() { return m_Wrapper.m_Menus; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(MenusActions set) { return set.Get(); }
        public void SetCallbacks(IMenusActions instance)
        {
            if (m_Wrapper.m_MenusActionsCallbackInterface != null)
            {
                Menu1.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu1;
                Menu1.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu1;
                Menu1.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu1;
                Menu2.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu2;
                Menu2.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu2;
                Menu2.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu2;
                Menu3.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu3;
                Menu3.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu3;
                Menu3.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu3;
                Menu4.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu4;
                Menu4.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu4;
                Menu4.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnMenu4;
                Back.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnBack;
                Back.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnBack;
                Back.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnBack;
                Help.started -= m_Wrapper.m_MenusActionsCallbackInterface.OnHelp;
                Help.performed -= m_Wrapper.m_MenusActionsCallbackInterface.OnHelp;
                Help.canceled -= m_Wrapper.m_MenusActionsCallbackInterface.OnHelp;
            }
            m_Wrapper.m_MenusActionsCallbackInterface = instance;
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
                Help.started += instance.OnHelp;
                Help.performed += instance.OnHelp;
                Help.canceled += instance.OnHelp;
            }
        }
    }
    public MenusActions @Menus
    {
        get
        {
            return new MenusActions(this);
        }
    }
    public interface IMenusActions
    {
        void OnMenu1(InputAction.CallbackContext context);
        void OnMenu2(InputAction.CallbackContext context);
        void OnMenu3(InputAction.CallbackContext context);
        void OnMenu4(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnHelp(InputAction.CallbackContext context);
    }
}
