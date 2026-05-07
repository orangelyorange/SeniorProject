using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SCRIPTS.ITEM_SCRIPT
{
    public class JournalManager : MonoBehaviour
    {
        public static JournalManager Instance;
       
        [Header("Main Journal UI")]
        public GameObject journalUIPanel;
        public Transform scrollViewContent; //where entries are spawned
        public GameObject journalEntryPrefab; //visual template for entry
        [SerializeField] private GameObject scrollViewPanel;
        [SerializeField] private TextMeshProUGUI pageTitleText;
        [SerializeField] private TextMeshProUGUI pageBodyText;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private string pageTitleObjectName = "Lore";
        [SerializeField] private string pageBodyObjectName = "Prayer";

        [Header("Tab UI")]
        [SerializeField] private GameObject bookmarkTemplate;
        [SerializeField] private Sprite act1BookmarkSprite;
        [SerializeField] private Sprite act2BookmarkSprite;
        [SerializeField] private Sprite settingsBookmarkSprite;
        [SerializeField] private Sprite inventoryBookmarkSprite;

        [Header("Pop-up Journal UI")]
        public GameObject PopUpPanel;
        public TextMeshProUGUI popUpTitle;
        public TextMeshProUGUI popUpContent;

        private JournalTab activeTab = JournalTab.Act1;
        private bool tabsInitialized;

       private void Awake()
       {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            EnsureReferences();

            if (journalUIPanel != null) journalUIPanel.SetActive(false);

            if (PopUpPanel != null) PopUpPanel.SetActive(false);

            JournalProgressManager.GetOrCreate();
        }

        private void OnEnable()
        {
            if (JournalProgressManager.Instance != null)
            {
                JournalProgressManager.Instance.JournalUpdated += HandleJournalUpdated;
            }
        }

        private void OnDisable()
        {
            if (JournalProgressManager.Instance != null)
            {
                JournalProgressManager.Instance.JournalUpdated -= HandleJournalUpdated;
            }
        }

        private void Start()
        {
            EnsureTabButtons();
            SetTab(activeTab);
        }

        private void Update()
        {
            //calls when J is pressed
            if (Input.GetKeyDown(KeyCode.J) && (PopUpPanel == null || !PopUpPanel.activeSelf))
            {
                ToggleJournal();
            }
           
           if (PopUpPanel != null && PopUpPanel.activeSelf)
           {
               if (Input.GetMouseButtonDown(0))
               {
                   ClosePopUp();
               }
           }
       }
       
        //calls when Journal item is picked up
        public void AddNewLore(JournalCollectedPage page)
        {
            if (page == null)
            {
                return;
            }

            JournalProgressManager progressManager = JournalProgressManager.GetOrCreate();
            if (!progressManager.TryAddPage(page))
            {
                return;
            }

            ShowPopUp(page.title, page.body);

            if (journalUIPanel != null && journalUIPanel.activeSelf && IsActTab(activeTab))
            {
                RefreshJournalEntries();
                SelectPage(page);
            }
        }

        //handles the turning on of the pop up panel and filling in the text
        private void ShowPopUp(string title, string content)
        {
               if (PopUpPanel != null)
               {
                popUpTitle.text = title;
                popUpContent.text = content;
                PopUpPanel.SetActive(true);
                
                //freezes game so player can read it
                Time.timeScale = 0;
              }
       }

       private void ClosePopUp()
       {
           if (PopUpPanel != null)
           {
                PopUpPanel.SetActive(false);
                
                //resumes game
                if (journalUIPanel == null || !journalUIPanel.activeSelf) //only resumes if journal isn't open
                {
                    Time.timeScale = 1;
                }
            }
        }

        //Toggles the panel on and off 
        public void ToggleJournal()
        {
            //flips active state of the panel
            if (journalUIPanel == null)
            {
                return;
            }

            journalUIPanel.SetActive(!journalUIPanel.activeSelf);

            if (journalUIPanel.activeSelf)
            {
                SetTab(activeTab);
            }

            //if the panel is now active, pause the game; if it's closed, resume the game
            Time.timeScale = journalUIPanel.activeSelf ? 0f : 1f;
        }

        public void SetTab(JournalTab tab)
        {
            activeTab = tab;

            if (IsActTab(tab))
            {
                SetPanelActive(settingsPanel, false);
                SetPanelActive(inventoryPanel, false);
                SetPanelActive(scrollViewPanel, true);
                RefreshJournalEntries();
            }
            else
            {
                SetPanelActive(scrollViewPanel, false);
                SetPanelActive(settingsPanel, tab == JournalTab.Settings);
                SetPanelActive(inventoryPanel, tab == JournalTab.Inventory);
                ShowPlaceholder(tab);
            }
        }

        private void HandleJournalUpdated()
        {
            if (journalUIPanel != null && journalUIPanel.activeSelf && IsActTab(activeTab))
            {
                RefreshJournalEntries();
            }
        }

        private void RefreshJournalEntries()
        {
            if (scrollViewContent == null || journalEntryPrefab == null)
            {
                return;
            }

            foreach (Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }

            List<JournalCollectedPage> pages = JournalProgressManager.GetOrCreate().GetCollectedPages(activeTab);
            if (pages.Count == 0)
            {
                ShowEmptyPageMessage();
                return;
            }

            foreach (JournalCollectedPage page in pages)
            {
                GameObject newEntry = Instantiate(journalEntryPrefab, scrollViewContent);
                JournalEntry entryScript = newEntry.GetComponent<JournalEntry>();
                if (entryScript != null)
                {
                    entryScript.Setup(page, SelectPage);
                }
            }

            SelectPage(pages[0]);
        }

        private void SelectPage(JournalCollectedPage page)
        {
            if (pageTitleText != null)
            {
                pageTitleText.text = page?.title ?? string.Empty;
            }

            if (pageBodyText != null)
            {
                pageBodyText.text = page?.body ?? string.Empty;
            }
        }

        private void ShowEmptyPageMessage()
        {
            if (pageTitleText != null)
            {
                pageTitleText.text = "No entries collected";
            }

            if (pageBodyText != null)
            {
                pageBodyText.text = "Collect journal pages to see them here.";
            }
        }

        private void ShowPlaceholder(JournalTab tab)
        {
            if (pageTitleText == null || pageBodyText == null)
            {
                return;
            }

            switch (tab)
            {
                case JournalTab.Settings:
                    pageTitleText.text = "Settings";
                    pageBodyText.text = "Settings content appears here.";
                    break;
                case JournalTab.Inventory:
                    pageTitleText.text = "Inventory";
                    pageBodyText.text = "Inventory content appears here.";
                    break;
            }
        }

        private bool IsActTab(JournalTab tab)
        {
            return tab == JournalTab.Act1 || tab == JournalTab.Act2;
        }

        private void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
            {
                panel.SetActive(isActive);
            }
        }

        private void EnsureReferences()
        {
            if (journalUIPanel == null)
            {
                journalUIPanel = GameObject.Find("Journal Panel");
            }

            if (scrollViewContent == null && journalUIPanel != null)
            {
                ScrollRect scrollRect = journalUIPanel.GetComponentInChildren<ScrollRect>(true);
                if (scrollRect != null)
                {
                    scrollViewContent = scrollRect.content;
                    scrollViewPanel = scrollRect.gameObject;
                }
            }

            if (scrollViewPanel == null && scrollViewContent != null)
            {
                scrollViewPanel = scrollViewContent.gameObject;
            }

            if (pageTitleText == null)
            {
                pageTitleText = FindTextInPanel(pageTitleObjectName);
            }

            if (pageBodyText == null)
            {
                pageBodyText = FindTextInPanel(pageBodyObjectName);
            }

            if (bookmarkTemplate == null && journalUIPanel != null)
            {
                Transform bookmarkTransform = FindChildByName(journalUIPanel.transform, "Bookmark");
                if (bookmarkTransform != null)
                {
                    bookmarkTemplate = bookmarkTransform.gameObject;
                }
            }
        }

        private void EnsureTabButtons()
        {
            if (tabsInitialized || bookmarkTemplate == null)
            {
                return;
            }

            RectTransform templateRect = bookmarkTemplate.GetComponent<RectTransform>();
            if (templateRect == null || templateRect.parent == null)
            {
                return;
            }

            List<(JournalTab tab, string label, Sprite sprite)> tabDefinitions = new List<(JournalTab, string, Sprite)>
            {
                (JournalTab.Act1, "Act 1", act1BookmarkSprite),
                (JournalTab.Act2, "Act 2", act2BookmarkSprite),
                (JournalTab.Settings, "Settings", settingsBookmarkSprite),
                (JournalTab.Inventory, "Inventory", inventoryBookmarkSprite)
            };

            float spacing = templateRect.rect.height;
            if (spacing <= 0f)
            {
                spacing = templateRect.sizeDelta.y;
            }

            if (spacing <= 0f)
            {
                spacing = 60f;
            }

            float offsetY = spacing + 10f;
            Vector2 basePosition = templateRect.anchoredPosition;
            Transform parent = templateRect.parent;

            for (int index = 0; index < tabDefinitions.Count; index++)
            {
                (JournalTab tab, string label, Sprite sprite) definition = tabDefinitions[index];
                GameObject bookmarkObject = index == 0
                    ? bookmarkTemplate
                    : CreateBookmarkClone(parent, bookmarkTemplate, definition.tab, definition.label);

                RectTransform bookmarkRect = bookmarkObject.GetComponent<RectTransform>();
                if (bookmarkRect != null)
                {
                    bookmarkRect.anchoredPosition = new Vector2(basePosition.x, basePosition.y - (offsetY * index));
                }

                UpdateBookmarkVisuals(bookmarkObject, definition.label, definition.sprite);
                AttachTabButton(bookmarkObject, definition.tab, definition.label);
            }

            tabsInitialized = true;
        }

        private GameObject CreateBookmarkClone(Transform parent, GameObject template, JournalTab tab, string label)
        {
            Transform existing = FindChildByName(parent, $"Bookmark {tab}");
            if (existing != null)
            {
                return existing.gameObject;
            }

            GameObject clone = Instantiate(template, parent);
            clone.name = $"Bookmark {tab}";
            return clone;
        }

        private void UpdateBookmarkVisuals(GameObject bookmarkObject, string labelText, Sprite spriteOverride)
        {
            if (bookmarkObject == null)
            {
                return;
            }

            TextMeshProUGUI label = bookmarkObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.text = labelText;
            }

            Image image = bookmarkObject.GetComponent<Image>();
            if (image == null)
            {
                image = bookmarkObject.GetComponentInChildren<Image>(true);
            }

            if (image != null && spriteOverride != null)
            {
                image.sprite = spriteOverride;
            }
        }

        private void AttachTabButton(GameObject bookmarkObject, JournalTab tab, string label)
        {
            if (bookmarkObject == null)
            {
                return;
            }

            JournalTabButton tabButton = bookmarkObject.GetComponent<JournalTabButton>();
            if (tabButton == null)
            {
                tabButton = bookmarkObject.AddComponent<JournalTabButton>();
            }

            tabButton.Initialize(tab, label);
        }

        private TextMeshProUGUI FindTextInPanel(string objectName)
        {
            if (journalUIPanel == null)
            {
                return null;
            }

            foreach (TextMeshProUGUI text in journalUIPanel.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (text.gameObject.name == objectName)
                {
                    return text;
                }
            }

            return null;
        }

        private Transform FindChildByName(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }
    }
}
