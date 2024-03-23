//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/PlayerInput/InteractionsMap/PlayerInput.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""833856fb-748b-4d4d-bec4-275c74e5018a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""941620e2-9e1a-463c-9848-692a50c66eb6"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""2a88a4a5-0bb4-4daa-bc87-9b526a0291af"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""6ea16cfb-730b-4323-8cd2-1b83f9135dd3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""9755774f-8efb-4196-9f9d-7abdae77a9fd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""a6a0d13a-f36c-4bc9-b9bf-acf01c278247"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""77866b69-8eab-4607-b071-3956ea617ae2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PickUpItem"",
                    ""type"": ""Button"",
                    ""id"": ""cb642db9-40c8-4828-bb76-d41c02d5f201"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PlaceItem"",
                    ""type"": ""Button"",
                    ""id"": ""73918d3c-dca1-466c-9818-bc1801c892b8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""DropItem"",
                    ""type"": ""Button"",
                    ""id"": ""d38466f0-2ef7-47fe-8781-a91e1ea7e1bd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ScrollWheelY"",
                    ""type"": ""Value"",
                    ""id"": ""66b1a4ac-5e5b-420c-a558-81ef94fc13df"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Hotbar 1"",
                    ""type"": ""Button"",
                    ""id"": ""5172efa0-1d64-4ebf-bb22-3ad09ff41248"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar 2"",
                    ""type"": ""Button"",
                    ""id"": ""956f25fc-9653-4bc6-a846-b7f0daba5fc4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar 3"",
                    ""type"": ""Button"",
                    ""id"": ""a219cbf6-713f-472e-9021-0ea1582195f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""FlahsLight"",
                    ""type"": ""Button"",
                    ""id"": ""54d6d703-1eb2-4085-bb7b-c6e66da1bf97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AnyKey"",
                    ""type"": ""Button"",
                    ""id"": ""435782f9-ce92-493e-ac83-a31471976dbf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Hotbar 4"",
                    ""type"": ""Button"",
                    ""id"": ""62a5ebb4-3efd-4871-a0b9-49ce8180167d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0d3f97d5-f12c-47dd-9878-967ecf0a5289"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""492d3bc1-4026-445e-8192-dc45aa285fa1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""66511fb8-5cb5-4673-b64c-9e5c0ad5b676"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""34f33302-6edb-402e-a8b8-0e59e4c5c6bd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b7f2fd56-fd3e-469f-9381-745b94cd0672"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""321cdf22-33f1-4673-9f54-5e9b3f5311fa"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""42dd6b42-a056-4d76-a82c-cf05ad68739f"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0fc38ae-2e7f-4e95-9bc0-e196f8a2d002"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be1f4156-16e7-415a-bb6d-98a9b9e0ae0d"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""DropItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""88f26189-1aba-4606-a833-f4f1c48e31f5"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Hotbar 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bcca3cad-2a91-444d-ab25-60d10aaf3683"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Hotbar 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd1ecb54-e277-4da9-8079-e874c9514041"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Hotbar 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7e87fa23-12c7-4a7b-ba65-900502a6aa9b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5cb3cd4-90f5-4f1a-92d1-6527d1a43f4d"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollWheelY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f15f03d-6efa-422d-bac1-155a7bff1cdf"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PlaceItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82e08f9e-4b52-4351-8045-834407c9d467"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71302012-a126-4522-abb4-77e5e5d715eb"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PickUpItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd1d8848-78e7-4797-88fd-bd567af66808"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9963cd74-56c1-4b92-b8ac-e135bf9f465c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ae8f185-4417-42ed-9964-48f99422e7b8"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76c047c7-cf7c-4602-b46f-c068ca316b5b"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""FlahsLight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23fa90c5-ddd7-4fcc-b92b-d37810885a76"",
                    ""path"": ""<Keyboard>/anyKey"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""AnyKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""14cfde13-b98f-401f-825c-71ae29160eb8"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Hotbar 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""900fec75-8241-43ea-9991-1853a4bab231"",
            ""actions"": [
                {
                    ""name"": ""PauseMenu"",
                    ""type"": ""Button"",
                    ""id"": ""4432731b-6563-41de-9216-27a44cc242bd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""NoteBook"",
                    ""type"": ""Button"",
                    ""id"": ""8a87231b-0e7c-4459-877c-8b3c99278cd6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3ebc75a2-0025-438d-bf06-2b390ebc2fe8"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PauseMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a4c9b2e6-3df2-425a-b029-71d8b9162c78"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""NoteBook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_UseItem = m_Player.FindAction("UseItem", throwIfNotFound: true);
        m_Player_PickUpItem = m_Player.FindAction("PickUpItem", throwIfNotFound: true);
        m_Player_PlaceItem = m_Player.FindAction("PlaceItem", throwIfNotFound: true);
        m_Player_DropItem = m_Player.FindAction("DropItem", throwIfNotFound: true);
        m_Player_ScrollWheelY = m_Player.FindAction("ScrollWheelY", throwIfNotFound: true);
        m_Player_Hotbar1 = m_Player.FindAction("Hotbar 1", throwIfNotFound: true);
        m_Player_Hotbar2 = m_Player.FindAction("Hotbar 2", throwIfNotFound: true);
        m_Player_Hotbar3 = m_Player.FindAction("Hotbar 3", throwIfNotFound: true);
        m_Player_FlahsLight = m_Player.FindAction("FlahsLight", throwIfNotFound: true);
        m_Player_AnyKey = m_Player.FindAction("AnyKey", throwIfNotFound: true);
        m_Player_Hotbar4 = m_Player.FindAction("Hotbar 4", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_PauseMenu = m_UI.FindAction("PauseMenu", throwIfNotFound: true);
        m_UI_NoteBook = m_UI.FindAction("NoteBook", throwIfNotFound: true);
    }

    public void Dispose()
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

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_PickUpItem;
    private readonly InputAction m_Player_PlaceItem;
    private readonly InputAction m_Player_DropItem;
    private readonly InputAction m_Player_ScrollWheelY;
    private readonly InputAction m_Player_Hotbar1;
    private readonly InputAction m_Player_Hotbar2;
    private readonly InputAction m_Player_Hotbar3;
    private readonly InputAction m_Player_FlahsLight;
    private readonly InputAction m_Player_AnyKey;
    private readonly InputAction m_Player_Hotbar4;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @PickUpItem => m_Wrapper.m_Player_PickUpItem;
        public InputAction @PlaceItem => m_Wrapper.m_Player_PlaceItem;
        public InputAction @DropItem => m_Wrapper.m_Player_DropItem;
        public InputAction @ScrollWheelY => m_Wrapper.m_Player_ScrollWheelY;
        public InputAction @Hotbar1 => m_Wrapper.m_Player_Hotbar1;
        public InputAction @Hotbar2 => m_Wrapper.m_Player_Hotbar2;
        public InputAction @Hotbar3 => m_Wrapper.m_Player_Hotbar3;
        public InputAction @FlahsLight => m_Wrapper.m_Player_FlahsLight;
        public InputAction @AnyKey => m_Wrapper.m_Player_AnyKey;
        public InputAction @Hotbar4 => m_Wrapper.m_Player_Hotbar4;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @Crouch.started += instance.OnCrouch;
            @Crouch.performed += instance.OnCrouch;
            @Crouch.canceled += instance.OnCrouch;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @UseItem.started += instance.OnUseItem;
            @UseItem.performed += instance.OnUseItem;
            @UseItem.canceled += instance.OnUseItem;
            @PickUpItem.started += instance.OnPickUpItem;
            @PickUpItem.performed += instance.OnPickUpItem;
            @PickUpItem.canceled += instance.OnPickUpItem;
            @PlaceItem.started += instance.OnPlaceItem;
            @PlaceItem.performed += instance.OnPlaceItem;
            @PlaceItem.canceled += instance.OnPlaceItem;
            @DropItem.started += instance.OnDropItem;
            @DropItem.performed += instance.OnDropItem;
            @DropItem.canceled += instance.OnDropItem;
            @ScrollWheelY.started += instance.OnScrollWheelY;
            @ScrollWheelY.performed += instance.OnScrollWheelY;
            @ScrollWheelY.canceled += instance.OnScrollWheelY;
            @Hotbar1.started += instance.OnHotbar1;
            @Hotbar1.performed += instance.OnHotbar1;
            @Hotbar1.canceled += instance.OnHotbar1;
            @Hotbar2.started += instance.OnHotbar2;
            @Hotbar2.performed += instance.OnHotbar2;
            @Hotbar2.canceled += instance.OnHotbar2;
            @Hotbar3.started += instance.OnHotbar3;
            @Hotbar3.performed += instance.OnHotbar3;
            @Hotbar3.canceled += instance.OnHotbar3;
            @FlahsLight.started += instance.OnFlahsLight;
            @FlahsLight.performed += instance.OnFlahsLight;
            @FlahsLight.canceled += instance.OnFlahsLight;
            @AnyKey.started += instance.OnAnyKey;
            @AnyKey.performed += instance.OnAnyKey;
            @AnyKey.canceled += instance.OnAnyKey;
            @Hotbar4.started += instance.OnHotbar4;
            @Hotbar4.performed += instance.OnHotbar4;
            @Hotbar4.canceled += instance.OnHotbar4;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @Crouch.started -= instance.OnCrouch;
            @Crouch.performed -= instance.OnCrouch;
            @Crouch.canceled -= instance.OnCrouch;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @UseItem.started -= instance.OnUseItem;
            @UseItem.performed -= instance.OnUseItem;
            @UseItem.canceled -= instance.OnUseItem;
            @PickUpItem.started -= instance.OnPickUpItem;
            @PickUpItem.performed -= instance.OnPickUpItem;
            @PickUpItem.canceled -= instance.OnPickUpItem;
            @PlaceItem.started -= instance.OnPlaceItem;
            @PlaceItem.performed -= instance.OnPlaceItem;
            @PlaceItem.canceled -= instance.OnPlaceItem;
            @DropItem.started -= instance.OnDropItem;
            @DropItem.performed -= instance.OnDropItem;
            @DropItem.canceled -= instance.OnDropItem;
            @ScrollWheelY.started -= instance.OnScrollWheelY;
            @ScrollWheelY.performed -= instance.OnScrollWheelY;
            @ScrollWheelY.canceled -= instance.OnScrollWheelY;
            @Hotbar1.started -= instance.OnHotbar1;
            @Hotbar1.performed -= instance.OnHotbar1;
            @Hotbar1.canceled -= instance.OnHotbar1;
            @Hotbar2.started -= instance.OnHotbar2;
            @Hotbar2.performed -= instance.OnHotbar2;
            @Hotbar2.canceled -= instance.OnHotbar2;
            @Hotbar3.started -= instance.OnHotbar3;
            @Hotbar3.performed -= instance.OnHotbar3;
            @Hotbar3.canceled -= instance.OnHotbar3;
            @FlahsLight.started -= instance.OnFlahsLight;
            @FlahsLight.performed -= instance.OnFlahsLight;
            @FlahsLight.canceled -= instance.OnFlahsLight;
            @AnyKey.started -= instance.OnAnyKey;
            @AnyKey.performed -= instance.OnAnyKey;
            @AnyKey.canceled -= instance.OnAnyKey;
            @Hotbar4.started -= instance.OnHotbar4;
            @Hotbar4.performed -= instance.OnHotbar4;
            @Hotbar4.canceled -= instance.OnHotbar4;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_PauseMenu;
    private readonly InputAction m_UI_NoteBook;
    public struct UIActions
    {
        private @PlayerInput m_Wrapper;
        public UIActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseMenu => m_Wrapper.m_UI_PauseMenu;
        public InputAction @NoteBook => m_Wrapper.m_UI_NoteBook;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @PauseMenu.started += instance.OnPauseMenu;
            @PauseMenu.performed += instance.OnPauseMenu;
            @PauseMenu.canceled += instance.OnPauseMenu;
            @NoteBook.started += instance.OnNoteBook;
            @NoteBook.performed += instance.OnNoteBook;
            @NoteBook.canceled += instance.OnNoteBook;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @PauseMenu.started -= instance.OnPauseMenu;
            @PauseMenu.performed -= instance.OnPauseMenu;
            @PauseMenu.canceled -= instance.OnPauseMenu;
            @NoteBook.started -= instance.OnNoteBook;
            @NoteBook.performed -= instance.OnNoteBook;
            @NoteBook.canceled -= instance.OnNoteBook;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnPickUpItem(InputAction.CallbackContext context);
        void OnPlaceItem(InputAction.CallbackContext context);
        void OnDropItem(InputAction.CallbackContext context);
        void OnScrollWheelY(InputAction.CallbackContext context);
        void OnHotbar1(InputAction.CallbackContext context);
        void OnHotbar2(InputAction.CallbackContext context);
        void OnHotbar3(InputAction.CallbackContext context);
        void OnFlahsLight(InputAction.CallbackContext context);
        void OnAnyKey(InputAction.CallbackContext context);
        void OnHotbar4(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnPauseMenu(InputAction.CallbackContext context);
        void OnNoteBook(InputAction.CallbackContext context);
    }
}
