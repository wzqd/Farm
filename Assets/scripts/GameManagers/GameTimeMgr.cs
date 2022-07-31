using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeMgr : SingletonMono<GameTimeMgr>
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.Spring;

    public bool gameClockPause;
    private float secondTimer;

    private void Awake()
    {
        NewGameTime();
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            secondTimer += Time.deltaTime;

            if (secondTimer >= Settings.secondThreshold)
            {
                secondTimer -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2022;
        gameSeason = Season.Spring;
    }

    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond <= Settings.secondHold) return;
        gameMinute++;
        gameSecond = 0;

        if (gameMinute <= Settings.minuteHold) return;
        gameHour++;
        gameMinute = 0;

        if (gameHour <= Settings.hourHold) return;
        gameDay++;
        gameHour = 7;

        if (gameDay <= Settings.dayHold) return;
        gameDay = 1;
        gameMonth++;

        if (gameMonth > 12)
            gameMonth = 1;

        int seasonNumber = (int)gameSeason;
        seasonNumber++;

        if (seasonNumber > Settings.seasonHold)
        {
            seasonNumber = 0;
            gameYear++;
        }

        gameSeason = (Season)seasonNumber;

        if (gameYear > 9999)
        {
            gameYear = 2022;
        }
    }
}
