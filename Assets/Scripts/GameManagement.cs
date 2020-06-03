using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour {

    private AudioSource gameMusic;
    private readonly Queue<float> beats = new Queue<float>();
    // inin , this for the atkBeats and there answer sheet;
    private readonly Queue<float> atkBeats = new Queue<float>();
    [SerializeField]
    GameObject SwordPahtObj;

    private readonly Queue<PlayerAction> sols = new Queue<PlayerAction>();
    // move from ShowTransform to here
    public Queue<PlayerAction> detectedActions;

    // public GameObject showTransformCarrier;
    // private ShowTransform showTransform;

    public GameObject sampleLightCarrier;
    private Light sampleLight;
    private readonly Color[] colorsForBeats = {Color.yellow, Color.cyan, Color.magenta};
    private int colorsIndex;

    public GameObject player;
    public GameObject bossAttackMarker;
    private readonly Dictionary<GameObject, GameCoordinate> objectMap =
        new Dictionary<GameObject, GameCoordinate>();

    // Show all possible positions
    public GameObject coordinateMark;

    // 張文胤
    [SerializeField] private float moveSpeed;
    [SerializeField] private float height;

    // 跟生命值有關的屬性
    [SerializeField] private float maxPlayerHealth;
    private float playerHealth;
    [SerializeField] private float maxBossHealth;
    private float bossHealth;
    //public Text healthStatusText;


    // 洪冠群

    // 歌曲相關
    // 產生音符的GameObject
    public BeatPool beatPool;
    private float beatSpawnEarly;
    // 生命相關
    public HealthControl hpController;


    private void Awake()
    {

    }

    private void Start() {


        detectedActions = new Queue<PlayerAction>();
        // set beat spawn delay
        beatSpawnEarly = beatPool.GetSpawnEarly();

        gameMusic = GetComponent<AudioSource>();
        gameMusic.loop = true;
        // if (!gameMusic.playOnAwake) gameMusic.Play();

        //showTransform = showTransformCarrier.GetComponent<ShowTransform>();
        sampleLight = sampleLightCarrier.GetComponent<Light>();

        // 測試用，音樂完成後應替換成真實資料（單位：秒）// Test, change 5 to 2, change 60 to 200
        /* for (float i = 5; i <= 200; i += 2)
        {
            beats.Enqueue(i - 0.2f);
            atkBeats.Enqueue(i - 0.5f);
        } */

        // 各個拍點分別需要從哪個方向攻擊敵人
        while (sols.Count < beats.Count) {
            // 隨機產生一個方向，相同方向重覆八次（僅供參考，歡迎修改）
            var nextAction = (PlayerAction) Random.Range(
                (int) PlayerAction.AttackUp, (int) PlayerAction.AttackRight + 1
            );
            for (var _ = 0; _ < 8; _++)
                sols.Enqueue(nextAction);
        }

        var initialPosition = new GameCoordinate(5, 0);
        objectMap.Add(player, initialPosition);
        player.transform.position = initialPosition.ToCartesianCoordinate(height);
        player.transform.LookAt(Vector3.zero);

        // Show all possible positions
        ShowAllPosition();

        playerHealth = maxPlayerHealth;
        bossHealth = maxBossHealth;
        InvokeRepeating(nameof(RecoverHealth), 0.5f, 0.5f);
        UpdateUI();

    }

    // Show all possible positions
    private void ShowAllPosition() {
        for (var i = 0; i < GameCoordinate.GetSizeOfM(); ++i) {
            for (var j = 0; j < GameCoordinate.GetSizeOfN(); ++j) {
                var position = new GameCoordinate(i, j);
                var newCoordinateMark = Instantiate(
                    coordinateMark,
                    position.ToCartesianCoordinate(),
                    coordinateMark.transform.rotation,
                    coordinateMark.transform.parent
                );
                newCoordinateMark.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            playerHealth -= 1;
    }

    private void FixedUpdate() {

        // UI show beat, by 冠群
        if (beats.Count != 0 && gameMusic.time >= beats.Peek()-beatSpawnEarly)
        {
            beats.Dequeue();
            // 1 is from right to left，移動我預設是左到右
            beatPool.ReuseByDirection(1);
        }
        // UI show atkBeats, by 文胤
        if (atkBeats.Count != 0 && gameMusic.time >= atkBeats.Peek() - beatSpawnEarly)
        {
            float time =  atkBeats.Dequeue();
            // -1 is from left to right，攻擊我預設是右到左
            beatPool.ReuseByDirection(-1);
            GameObject obj = Instantiate(SwordPahtObj, Vector3.zero, Quaternion.identity);
            SwordPathGen temp = new SwordPathGen(time);
            obj.GetComponent<SwordPath>().SetAnswer(temp.GetAnswer());
        }
        /*
        if (beats.Count != 0 && gameMusic.time >= beats.Peek()) {
            // 下一個拍點到了
            beats.Dequeue();
            // 啟動玩家動作偵測
            showTransform.turnedOn = true;
            // 變換燈光顏色（示範）
            sampleLight.color = colorsForBeats[colorsIndex];
            colorsIndex = (colorsIndex + 1) % 3;
        }
        */
        if (detectedActions.Count != 0) {
            // 處理玩家的動作
            GameCoordinate newPosition;
            var hurtableAction = (sols.Count != 0) ? sols.Dequeue() : PlayerAction.NoAction;
            // Note: sols.Count == 0 should not happen
            var playerAction = detectedActions.Dequeue();
            switch (playerAction) {
                case PlayerAction.MoveFront:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() - 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.MoveBack:
                    newPosition = new GameCoordinate(
                        objectMap[player].M() + 1, objectMap[player].N()
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.MoveLeft:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() - 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.MoveRight:
                    newPosition = new GameCoordinate(
                        objectMap[player].M(), objectMap[player].N() + 1
                    );
                    objectMap[player] = newPosition;
                    StartCoroutine(MoveSmoothly(newPosition.ToCartesianCoordinate(height)));
                    break;
                case PlayerAction.AttackUp:
                case PlayerAction.AttackDown:
                case PlayerAction.AttackLeft:
                case PlayerAction.AttackRight:
                    if (objectMap[player].M() == 0 && hurtableAction == playerAction) {
                        Debug.Log("敵人被攻擊！");
                        bossHealth -= 1;
                    }
                    break;
                case PlayerAction.NoAction:
                    break;
            }
            // 敵人移動現有攻擊、對玩家造成傷害、發動新攻擊
            BossAttack();
            //Debug.Log(ObjectMapToString(objectMap));
        }
        UpdateUI();
        if (bossHealth <= 0.0001) GameEnd(true);
        else if (playerHealth <= 0.0001) GameEnd(false);

    }

    private IEnumerator MoveSmoothly(Vector3 destination) {
        while (Vector3.Distance(player.transform.position, destination) > moveSpeed) {
            var direction = Vector3.Normalize(destination - player.transform.position);
            player.transform.Translate(direction * moveSpeed, Space.World);
            // 讓玩家的鏡頭前方永遠朝向中心點，好判斷跟計算
            player.transform.LookAt(Vector3.zero);
            yield return null;
        }
        player.transform.position = destination;
        player.transform.LookAt(Vector3.zero);
    }

    /**
     * 執行敵人的動作（每個拍點執行一次，先執行玩家動作，再執行敵人動作）
     * 敵人的動作包含 移動現有攻擊、對玩家造成傷害、發動新攻擊 三個部分
     * 1.移動現有攻擊：所有攻擊物件都移動到下一圈的同一格
     * 2.對玩家造成傷害：若玩家所在位置有攻擊物件，會減低玩家的生命值
     * 3.發動新攻擊：玩家在最外圈時生成 0 個攻擊物件，玩家每前進一步，生成的攻擊物件的數量加 1；
     *   　　　　　　隨機決定這些攻擊物件要落在第 -3 圈的哪幾格（玩家的活動範圍只到第 0 圈，攻擊物件可在第 0 圈之內）
     */
    private void BossAttack() {

        var objectMapKeys = new List<GameObject>(objectMap.Keys);
        var candidates = new List<int>();
        var numberOfNewAttack = GameCoordinate.GetSizeOfM() - objectMap[player].M() - 1;

        foreach (var key in objectMapKeys) {
            if (key == player) continue;
            var newPosition = new GameCoordinate(
                objectMap[key].M() + 1, objectMap[key].N(), true
            );
            if (newPosition.M() < GameCoordinate.GetSizeOfM()) {
                objectMap[key] = newPosition;
                key.transform.position = newPosition.ToCartesianCoordinate();
                var playerAttacked = newPosition.Equals(objectMap[player]);
                if (playerAttacked) {
                    Debug.Log("玩家被攻擊！");
                    playerHealth -= 1;
                }
            }
            else {
                objectMap.Remove(key);
                Destroy(key);
            }
        }

        for (var i = 0; i < GameCoordinate.GetSizeOfN(); i++)
            candidates.Add(i);

        for (var i = 0; i < numberOfNewAttack; i++) {
            var index = Random.Range(0, candidates.Count);
            var attackPosition = new GameCoordinate(-3, candidates[index], true);
            var newBossAttackMarker = Instantiate(
                bossAttackMarker,
                attackPosition.ToCartesianCoordinate(),
                Quaternion.identity
            );
            objectMap.Add(newBossAttackMarker, attackPosition);
            candidates.RemoveAt(index);
            if (candidates.Count == 0) break;
        }

    }

    private void RecoverHealth() {
        playerHealth += 0.1f;
        if (playerHealth > maxPlayerHealth)
            playerHealth = maxPlayerHealth;
        bossHealth += 0.1f;
        if (bossHealth > maxBossHealth)
            bossHealth = maxBossHealth;
    }

    private void UpdateUI() {
        hpController.SetPlayerHP(playerHealth);
        hpController.SetBossHP(bossHealth);
        /*
        var str = "Player Health: ";
        str += new string('▓', Mathf.RoundToInt(playerHealth / maxPlayerHealth * 10));
        str += "\nBoss Health: ";
        str += new string('▓', Mathf.RoundToInt(bossHealth / maxBossHealth * 10));
        str += "\n";
        if (sols.Count != 0) {
            str += "To Hurt Boss: ";
            switch (sols.Peek()) {
                case PlayerAction.AttackUp:
                    str += "↑";
                    break;
                case PlayerAction.AttackDown:
                    str += "↓";
                    break;
                case PlayerAction.AttackLeft:
                    str += "←";
                    break;
                case PlayerAction.AttackRight:
                    str += "→";
                    break;
            }
            str += "\n";
        }
        healthStatusText.text = str;
        */
    }

    private void GameEnd(bool playerWin) {
        /*
        var str = healthStatusText.text;
        str += "GAME ENDED! YOU ";
        str += playerWin ? "WIN" : "LOSE";
        str += "!\n";
        healthStatusText.text = str;
        */
        Time.timeScale = 0;
    }

    /**
     * 將屬性 objectMap 轉換成字串，用於除錯
     */
    private string ObjectMapToString(Dictionary<GameObject, GameCoordinate> dict) {
        var str = "objectMap[" + dict.Count + "] { ";
        foreach (var kvp in dict)
            str += kvp.Key.GetInstanceID() + ": " + kvp.Value + ", ";
        str += "}";
        return str;
    }


    // called by TrackChooser.cs
    public void SetTrack(AudioClip a)
    {
        gameMusic = GetComponent<AudioSource>();
        gameMusic.clip = a;
    }

    // called by TrackChooser.cs
    public void SetMoveCSV(string[] str)
    {
        beats.Clear();
        foreach (string s in str)
        {
            beats.Enqueue(float.Parse(s));
            Debug.Log(float.Parse(s));
        }
    }

    // called by TrackChooser.cs
    public void SetAttackCSV(string[] str)
    {
        atkBeats.Clear();
        foreach (string s in str)
        {
            atkBeats.Enqueue(float.Parse(s));
            Debug.Log(float.Parse(s));
        }
    }
}
