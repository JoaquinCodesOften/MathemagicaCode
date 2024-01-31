using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERT, ENEMYT, WON, LOST, FLED, WAITING}

public class BattleSystem : MonoBehaviour
{
    public SceneControl sc;

    //state change variables
    public BattleState state; 

    //battle unit variables
    public GameObject[] playerTeamPrefabs;
    public GameObject enemyPrefab;

    public Transform[] playerLocs;
    public Transform enemyLoc;

    public GameObject buttonsAccess;

    public List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    EnemyUnit enemyUnit;
    public Unit unitSelect; 

    private CombatPrefabRefer combatantReference;

    public BattleHUD[] playerHUDs;
    public BattleHUD enemyHUD;

    public Queue<Action> actionQueue = new Queue<Action>();
    public Action currentAction;
    public bool setUp;

    public Unit exampleUnit;
    public string exampleString;
    public Weapon exampleWeapon;

    //text display variables
    public Text dialogueText;

    //typing letter by letter variables
    //public bool isTyping;
    //public int maxVisibleChars;
    

    void Start()
    {
        state = BattleState.START;
        combatantReference = Resources.Load<CombatPrefabRefer>("SOs/CombatRefer");
        playerTeamPrefabs = combatantReference.allyTeam;
        enemyPrefab = combatantReference.enemyRefer;
        StartCoroutine(SetupBattle());
    }

    void Update() {
        if (state != BattleState.PLAYERT) {
            buttonsAccess.SetActive(false);
        } else {
            buttonsAccess.SetActive(true);
        }
    }

    // Instantiation and Set Up Coroutine
    IEnumerator SetupBattle()
    {
        int playerCount = 0;
        foreach (var playerPrefab in playerTeamPrefabs) {
            GameObject playGO = Instantiate(playerPrefab, playerLocs[playerCount]);
            playerCount += 1;
            playerUnits.Add(playGO.GetComponent<PlayerUnit>());
        }
        
        GameObject enGO = Instantiate(enemyPrefab, enemyLoc);
        enemyUnit = enGO.GetComponent<EnemyUnit>();

        dialogueText.text = "From the shadows, a " + enemyUnit.unitName + " has emerged!";

        playerCount = 0;
        foreach (var allyUnit in playerUnits) {
            playerHUDs[playerCount].gameObject.SetActive(true);
            playerHUDs[playerCount].SetHUD(allyUnit);
            playerCount += 1; 
        }

        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERT;
        setUp = true;
        PlayerTurn();
    }

    // PLAYER TURN OPTIONS 
    void PlayerTurn() {
        dialogueText.text = "Choose an action!"; 
    }

    public void onAttackButton(int weaponNum){
        int playerNum = actionQueue.Count;
        Action currentAction = new Action(playerUnits[playerNum].weaponList[weaponNum], enemyUnit, "ATTACK");
        actionQueue.Enqueue(currentAction);
        OnActionButton();
    }

    public void onCraftButton(int weaponNum){
        int playerNum = actionQueue.Count;
        Action currentAction = new Action(playerUnits[playerNum].weaponList[weaponNum], null, "CRAFT");
        actionQueue.Enqueue(currentAction);
        OnActionButton();
    }

    public void onFleeButton(){
        Action currentAction = new Action(null, null, "FLEE");
        actionQueue.Enqueue(currentAction);
        OnActionButton();
    }

    //ATTACK
    public void OnActionButton() {
        //Weapon weaponChoice, int target, string actionCategory
        if (state != BattleState.PLAYERT) {
            return;
        }

        if (actionQueue.Count == playerUnits.Count) {
            StartCoroutine (PlayerActs());
        } 
    }

    IEnumerator PlayerActs() {
        Debug.Log("Arrived");
        state = BattleState.WAITING;
        yield return new WaitForSeconds(1f);
        int queueCounter = 0;
        while (actionQueue.Count != 0) {
            bool isDead = playerUnits[queueCounter].executeAction(actionQueue.Dequeue());
            // Update VisualDamage to Enemy
            enemyHUD.SetHP(enemyUnit.currentHPReal);
            enemyHUD.HPSliderAngle(enemyUnit);
            queueCounter += 1;

            if (isDead) {
                state = BattleState.WON;
                StartCoroutine(EndBattle());
                yield break;
            } 
        }
        state = BattleState.ENEMYT;
        StartCoroutine(EnemyTurn());
    }

    //ENEMY OPTIONS
    IEnumerator EnemyTurn() {
        int randNum = Random.Range(0, 2);
        PlayerUnit playerUnit = playerUnits[randNum];
        state = BattleState.WAITING;
        dialogueText.text = enemyUnit.unitName + " attacks!"; 

        randNum = 1 - randNum;
        playerHUDs[randNum].setTop(playerHUDs[randNum].transform);

        yield return new WaitForSeconds(.5f);
        bool isDead = false;
        if (enemyUnit.numTurnsLeftSpecial == 0) {
            isDead = enemyUnit.specialAttack(playerUnit);
        } else {
            isDead = enemyUnit.standardAttack(playerUnit);
        }

        if (enemyUnit.numTurnsLeftStandardTwo == 0) {
            dialogueText.text = enemyUnit.secondStandardAttack(enemyUnit);
        } 

        randNum = 1 - randNum;
        playerHUDs[randNum].SetHP(playerUnit.currentHPReal);
        playerHUDs[randNum].HPSliderAngle(playerUnit);


        yield return new WaitForSeconds(2f);

        if (isDead) {
            state = BattleState.LOST;
            EndBattle();
        } else {
            state = BattleState.PLAYERT;
            PlayerTurn();
        }
    }

    //End Battle Conditions
    IEnumerator EndBattle() {
        if (state == BattleState.WON) {
            dialogueText.text = "You won the Battle!";
        } else if (state == BattleState.LOST) {
            dialogueText.text = "You lost the Battle...";
        } else if (state == BattleState.FLED) {
            dialogueText.text = "You fled the Battle...";
        }
        yield return new WaitForSeconds(2f);
        sc.CombatSceneUnload();
    }

    
    /*
    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;

        int maxVisibleChars = 0;

        NPCDialogueText.text = p;
        NPCDialogueText.maxVisibleCharacters = maxVisibleChars;        

        foreach (char c in p.ToCharArray())
        {

            maxVisibleChars++;
            NPCDialogueText.maxVisibleCharacters = maxVisibleChars;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }
    */
}

