using System.Collections;
using System.Collections.Generic;
using System;

/* how to use
 * SwordPathGen temp = new SwordPathGen( <YourTime> )
 * <YourAnsewer> = temp.GetAnswer()
 */

public class SwordPathGen
{
    // this indicate the previous atk beat time
    static float preTime;
    // this indicate the previous atk beat answer
    static int preDir;

    private int thisDir;

    public SwordPathGen(float time) {
        if (time - preTime <= 0.5f)
        {
            // 答案連動
            thisDir = (preDir + 6) % 12;
        }
        else
        {
            // 答案不連動，隨機
            Random rnd = new Random();
            thisDir = rnd.Next(0, 12);
        }

        preDir = thisDir;
        preTime = time;
    }

    public int GetAnswer() {
        return thisDir;
    }

}
