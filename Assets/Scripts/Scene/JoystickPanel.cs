using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace MarkovCraft
{
    /// <summary>
    /// UI control that turns mouse/touch drags into a 2D movement vector (dir + magnitude).
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class JoystickPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform handle;
        [SerializeField] private RectTransform background;
        [SerializeField] private Camera uiCameraOverride;
        [SerializeField] private Button zoomInButton;
        [SerializeField] private Button zoomOutButton;
        [SerializeField] private Button rotateLeftButton;
        [SerializeField] private Button rotateRightButton;

        [Header("Behavior")]
        [SerializeField] [Min(1F)] private float maxRadius = 120F;
        [SerializeField] [Range(0F, 1F)] private float deadZone = 0.1F;

        [Header("Events")]
        [SerializeField] private UnityEvent<Vector2> onValueChanged;

        public Vector2 Value { get; private set; }
        public bool IsHeld { get; private set; }
        public float Magnitude => Value.magnitude;
        
        public bool ZoomInButtonIsHeld { get; private set; }
        public bool ZoomOutButtonIsHeld { get; private set; }
        public bool RotateLeftButtonIsHeld { get; private set; }
        public bool RotateRightButtonIsHeld { get; private set; }

        public bool FlyUpButtonIsHeld { get; private set; }
        public bool FlyDownButtonIsHeld { get; private set; }

        private RectTransform rectTransform;
        private Canvas parentCanvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();

            CreateFlyButtons();

            ResetStick();
        }

        public void UpdateZoomInButtonHeldStatus(bool held)
        {
            ZoomInButtonIsHeld = held;
        }
        
        public void UpdateZoomOutButtonHeldStatus(bool held)
        {
            ZoomOutButtonIsHeld = held;
        }

        public void UpdateRotateLeftButtonHeldStatus(bool held)
        {
            RotateLeftButtonIsHeld = held;
        }

        public void UpdateRotateRightButtonHeldStatus(bool held)
        {
            RotateRightButtonIsHeld = held;
        }

        public void UpdateFlyUpButtonHeldStatus(bool held)
        {
            FlyUpButtonIsHeld = held;
        }

        public void UpdateFlyDownButtonHeldStatus(bool held)
        {
            FlyDownButtonIsHeld = held;
        }

        private void CreateFlyButtons()
        {
            var parent = transform;
            if (parent == null) return;

            // Fly Up button
            var flyUpGo = new GameObject("Fly-Up Button");
            flyUpGo.transform.SetParent(parent, false);
            var flyUpRect = flyUpGo.AddComponent<RectTransform>();
            flyUpRect.anchorMin = new Vector2(0.5F, 1);
            flyUpRect.anchorMax = new Vector2(0.5F, 1);
            flyUpRect.pivot = new Vector2(0.5F, 1);
            flyUpRect.anchoredPosition = new Vector2(0, -10);
            flyUpRect.sizeDelta = new Vector2(80, 60);
            var flyUpImg = flyUpGo.AddComponent<Image>();
            flyUpImg.color = new Color(1, 1, 1, 0.392F);
            var flyUpBtn = flyUpGo.AddComponent<Button>();
            flyUpBtn.targetGraphic = flyUpImg;
            var flyUpTrig = flyUpGo.AddComponent<EventTrigger>();
            var flyUpDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            flyUpDown.callback.AddListener((_) => UpdateFlyUpButtonHeldStatus(true));
            var flyUpUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            flyUpUp.callback.AddListener((_) => UpdateFlyUpButtonHeldStatus(false));
            flyUpTrig.triggers = new System.Collections.Generic.List<EventTrigger.Entry> { flyUpDown, flyUpUp };
            var flyUpTextGo = new GameObject("Text (TMP)");
            flyUpTextGo.transform.SetParent(flyUpGo.transform, false);
            var flyUpTextRect = flyUpTextGo.AddComponent<RectTransform>();
            flyUpTextRect.anchorMin = Vector2.zero;
            flyUpTextRect.anchorMax = Vector2.one;
            flyUpTextRect.sizeDelta = Vector2.zero;
            flyUpTextRect.pivot = new Vector2(0.5F, 0.5F);
            var flyUpText = flyUpTextGo.AddComponent<TMPro.TextMeshProUGUI>();
            flyUpText.text = "\u25B2";
            flyUpText.fontSize = 48;
            flyUpText.alignment = TMPro.TextAlignmentOptions.Center;
            flyUpText.color = new Color(0.2F, 0.2F, 0.2F, 0.784F);

            // Fly Down button
            var flyDownGo = new GameObject("Fly-Down Button");
            flyDownGo.transform.SetParent(parent, false);
            var flyDownRect = flyDownGo.AddComponent<RectTransform>();
            flyDownRect.anchorMin = new Vector2(0.5F, 1);
            flyDownRect.anchorMax = new Vector2(0.5F, 1);
            flyDownRect.pivot = new Vector2(0.5F, 1);
            flyDownRect.anchoredPosition = new Vector2(0, -80);
            flyDownRect.sizeDelta = new Vector2(80, 60);
            var flyDownImg = flyDownGo.AddComponent<Image>();
            flyDownImg.color = new Color(1, 1, 1, 0.392F);
            var flyDownBtn = flyDownGo.AddComponent<Button>();
            flyDownBtn.targetGraphic = flyDownImg;
            var flyDownTrig = flyDownGo.AddComponent<EventTrigger>();
            var flyDownDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            flyDownDown.callback.AddListener((_) => UpdateFlyDownButtonHeldStatus(true));
            var flyDownUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            flyDownUp.callback.AddListener((_) => UpdateFlyDownButtonHeldStatus(false));
            flyDownTrig.triggers = new System.Collections.Generic.List<EventTrigger.Entry> { flyDownDown, flyDownUp };
            var flyDownTextGo = new GameObject("Text (TMP)");
            flyDownTextGo.transform.SetParent(flyDownGo.transform, false);
            var flyDownTextRect = flyDownTextGo.AddComponent<RectTransform>();
            flyDownTextRect.anchorMin = Vector2.zero;
            flyDownTextRect.anchorMax = Vector2.one;
            flyDownTextRect.sizeDelta = Vector2.zero;
            flyDownTextRect.pivot = new Vector2(0.5F, 0.5F);
            var flyDownText = flyDownTextGo.AddComponent<TMPro.TextMeshProUGUI>();
            flyDownText.text = "\u25BC";
            flyDownText.fontSize = 48;
            flyDownText.alignment = TMPro.TextAlignmentOptions.Center;
            flyDownText.color = new Color(0.2F, 0.2F, 0.2F, 0.784F);
        }

        private Camera ResolveCamera()
        {
            if (uiCameraOverride) return uiCameraOverride;
            if (!parentCanvas) return null;
            return parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsHeld = true;
            if (TryGetLocalPoint(eventData, out var localPoint))
            {
                UpdateValue(localPoint);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsHeld) return;
            if (TryGetLocalPoint(eventData, out var localPoint))
            {
                UpdateValue(localPoint);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsHeld = false;
            ResetStick();
        }

        private bool TryGetLocalPoint(PointerEventData eventData, out Vector2 localPoint)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
                                                                           eventData.position,
                                                                           ResolveCamera(),
                                                                           out localPoint);
        }

        private void UpdateValue(Vector2 currentLocalPoint)
        {
            var rect = rectTransform.rect;
            // Keep pointer inside panel bounds
            var clampedPoint = new Vector2(
                Mathf.Clamp(currentLocalPoint.x, rect.xMin, rect.xMax),
                Mathf.Clamp(currentLocalPoint.y, rect.yMin, rect.yMax)
            );

            // Offset relative to center
            var offset = clampedPoint - rectTransform.rect.center;

            // Optional radial clamp to maxRadius to cap magnitude
            if (offset.sqrMagnitude > maxRadius * maxRadius)
            {
                offset = offset.normalized * maxRadius;
            }

            var newValue = offset; // Raw, not normalized

            // Dead zone expressed as fraction of maxRadius
            if (maxRadius > 0F && (newValue.magnitude / maxRadius) < deadZone)
            {
                newValue = Vector2.zero;
            }

            Value = newValue;
            onValueChanged?.Invoke(Value);

            if (handle)
            {
                handle.anchoredPosition = offset;
            }
        }

        private void ResetStick()
        {
            Value = Vector2.zero;
            onValueChanged?.Invoke(Value);

            if (handle)
            {
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }
}
