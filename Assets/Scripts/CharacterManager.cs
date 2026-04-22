using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupText;
    [SerializeField] private GameObject agustinIndicatorUI;

    private bool infrontOfPartyMember;
    private GameObject joinableMember;
    private PlayerControls playerControls;
    private List<GameObject> overworldCharacters = new List<GameObject>();

    private const string PARTY_JOINED_MESSAGE = " Joined The Party!";
    private const string NPC_JOINABLE_TAG = "NPCJoinable";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    // Start is called before the first frame update testing
    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        if (agustinIndicatorUI != null) agustinIndicatorUI.SetActive(false);
        SpawnOverworldMembers();
        
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void Interact()
    {
        if (infrontOfPartyMember == true && joinableMember != null)
        {
            MemberJoined(joinableMember.GetComponent<JoinableCharacterScript>().MemberToJoin);//add member
            infrontOfPartyMember = false;
            joinableMember = null;
        }
    }

    private void MemberJoined(PartyMemberInfo partyMember)
    {
        GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);// add party member
        joinableMember.GetComponent<JoinableCharacterScript>().CheckIfJoined();// disable joinable member
        // join pop up
        joinPopup.SetActive(true);
        joinPopupText.text = partyMember.MemberName + PARTY_JOINED_MESSAGE;

        if (partyMember.MemberName == "Soldier") 
        {
            if (agustinIndicatorUI != null) agustinIndicatorUI.SetActive(true);
        }

        SpawnOverworldMembers(); // adding an overworld member
    }

    private void SpawnOverworldMembers()
    {
        for (int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();

        PartyManager partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        if (partyManager == null)
        {
            Debug.LogWarning($"{nameof(CharacterManager)}: No PartyManager found, cannot spawn overworld members.");
            return;
        }

        List<PartyMember> currentParty = partyManager.GetCurrentParty();
        if (currentParty == null)
        {
            Debug.LogWarning($"{nameof(CharacterManager)}: currentParty is null on PartyManager.");
            return;
        }

        for (int i = 0; i < currentParty.Count; i++)
        {
            if (i == 0) // first member will be the player
            {
                GameObject player = gameObject; // get the player

                if (currentParty[i].MemberOverworldVisualPrefab == null)
                {
                    Debug.LogWarning($"{nameof(CharacterManager)}: Party member 0 has no overworld visual prefab.");
                    continue;
                }

                GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                transform.position, Quaternion.identity); // spawn the member visual

                playerVisual.transform.SetParent(player.transform, true); // settting the parent to the player
                playerVisual.transform.localPosition = Vector3.zero;

                Animator playerAnim = playerVisual.GetComponent<Animator>();
                if (playerAnim == null) playerAnim = playerVisual.GetComponentInChildren<Animator>();

                SpriteRenderer playerSprite = playerVisual.GetComponent<SpriteRenderer>();
                if (playerSprite == null) playerSprite = playerVisual.GetComponentInChildren<SpriteRenderer>();

                player.GetComponent<PlayerController>().SetOverworldVisuals(playerAnim,
                playerSprite, playerVisual.transform.localScale); // assign the player controller values
                // Player visual should not follow anything.
                MemberFollowAI playerFollow = playerVisual.GetComponent<MemberFollowAI>();
                if (playerFollow == null) playerFollow = playerVisual.GetComponentInChildren<MemberFollowAI>();
                if (playerFollow != null) playerFollow.enabled = false;
                overworldCharacters.Add(playerVisual); // add the overworld character visual to the list
            }
            else // any other will be a follower
            {
                Vector3 positionToSpawn = transform.position;// get the followers spawn position

                float spacing = 2.0f; 
                positionToSpawn.x -= i * spacing;


                if (currentParty[i].MemberOverworldVisualPrefab == null)
                {
                    Debug.LogWarning($"{nameof(CharacterManager)}: Party member {i} has no overworld visual prefab.");
                    continue;
                }

                GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                positionToSpawn, Quaternion.identity);// spawn the follower

                // Follower AI might be on the root or a child, and might be disabled on the prefab (e.g., Soldier).
                MemberFollowAI followerAI = tempFollower.GetComponent<MemberFollowAI>();
                if (followerAI == null) followerAI = tempFollower.GetComponentInChildren<MemberFollowAI>();
                if (followerAI != null)
                {
                    followerAI.enabled = true;
                    followerAI.SetFollowTarget(overworldCharacters[i - 1].transform);
                }
                overworldCharacters.Add(tempFollower); // add the follower visual to our list
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //enable our prompt
            infrontOfPartyMember = true;
            joinableMember = other.gameObject;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == NPC_JOINABLE_TAG)
        {
            //disable our prompt
            infrontOfPartyMember = false;
            joinableMember.GetComponent<JoinableCharacterScript>().ShowInteractPrompt(false);
            joinableMember = null;
        }
    }
}
