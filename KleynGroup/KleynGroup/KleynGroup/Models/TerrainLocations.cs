using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KleynGroup.Models
{
    public class TerrainLocations
    {
        //Trailers become orange, Trucks Blue, Vans Green in area color andd Combi terrain are Gray
        public readonly Color orangeRed = Color.FromRgba(255, 69, 0, 127);
        public readonly Color KleynOrange = Color.FromRgba(239, 118, 34, 127);

        public readonly Color KleynGray = Color.FromRgba(135, 120, 116, 127);
        public readonly Color DarkerGray = Color.FromRgba(195, 180, 176, 127);

        public readonly Color KleynGreen = Color.FromRgba(148, 187, 30, 127);
        public readonly Color SecondGreen = Color.FromRgba(120, 160, 15, 127);

        public readonly Color KleynBlue = Color.FromRgba(2, 31, 110, 127);
        public readonly Color SecondBlue = Color.FromRgba(20, 62, 153, 127);

        public readonly Color NormalKleynGray = Color.FromRgb(135, 120, 116);

        //Fridge Trailers
        public readonly double FTp1Latitude = 51.833718;
        public readonly double FTp1Longitude = 5.083141;

        public readonly double FTp2Latitude = 51.833748;
        public readonly double FTp2Longitude = 5.082653;

        public readonly double FTp3Latitude = 51.834792;
        public readonly double FTp3Longitude = 5.082906;

        public readonly double FTp4Latitude = 51.834812;
        public readonly double FTp4Longitude = 5.083377;

        //Schuifzeilen/Slidngssheets
        public readonly double SZp1Latitude = 51.833687;
        public readonly double SZp1Longitude = 5.083373;

        public readonly double SZp2Latitude = 51.833656;
        public readonly double SZp2Longitude = 5.083809;

        public readonly double SZp3Latitude = 51.834638;
        public readonly double SZp3Longitude = 5.083583;

        public readonly double SZp4Latitude = 51.834597;
        public readonly double SZp4Longitude = 5.083982;

        //Kippers/tippers
        public readonly double TIp1Latitude = 51.834597;
        public readonly double TIp1Longitude = 5.083982;

        public readonly double TIp2Latitude = 51.834638;
        public readonly double TIp2Longitude = 5.083583;

        public readonly double TIp3Latitude = 51.834776;
        public readonly double TIp3Longitude = 5.083633;

        public readonly double TIp4Latitude = 51.834776;
        public readonly double TIp4Longitude = 5.084017;

        //Tanks
        public readonly double TKp1Latitude = 51.834597;
        public readonly double TKp1Longitude = 5.083982;

        public readonly double TKp2Latitude = 51.834776;
        public readonly double TKp2Longitude = 5.084017;

        public readonly double TKp3Latitude = 51.834776;
        public readonly double TKp3Longitude = 5.084411;

        public readonly double TKp4Latitude = 51.834575;
        public readonly double TKp4Longitude = 5.084398;

        //Diepladers/Low loaders
        public readonly double LLp1Latitude = 51.834597;
        public readonly double LLp1Longitude = 5.083982;

        public readonly double LLp2Latitude = 51.834575;
        public readonly double LLp2Longitude = 5.084398;

        public readonly double LLp3Latitude = 51.833905;
        public readonly double LLp3Longitude = 5.084288;

        public readonly double LLp4Latitude = 51.833908;
        public readonly double LLp4Longitude = 5.083859;

        //remainder trailers
        public readonly double RTp1Latitude = 51.833908;
        public readonly double RTp1Longitude = 5.083859;

        public readonly double RTp2Latitude = 51.833656;
        public readonly double RTp2Longitude = 5.083809;

        public readonly double RTp3Latitude = 51.833636;
        public readonly double RTp3Longitude = 5.084209;

        public readonly double RTp4Latitude = 51.833905;
        public readonly double RTp4Longitude = 5.084288;

        //Container Transport
        public readonly double CTp1Latitude = 51.833608;
        public readonly double CTp1Longitude = 5.084474;

        public readonly double CTp2Latitude = 51.833591;
        public readonly double CTp2Longitude = 5.084872;

        public readonly double CTp3Latitude = 51.833820;
        public readonly double CTp3Longitude = 5.084932;

        public readonly double CTp4Latitude = 51.833861;
        public readonly double CTp4Longitude = 5.084557;

        //Combi Terrain 1
        public readonly double CB1p1Latitude = 51.833916;
        public readonly double CB1p1Longitude = 5.084968;

        public readonly double CB1p2Latitude = 51.833953;
        public readonly double CB1p2Longitude = 5.084581;

        public readonly double CB1p3Latitude = 51.834271;
        public readonly double CB1p3Longitude = 5.084675;

        public readonly double CB1p4Latitude = 51.834247;
        public readonly double CB1p4Longitude = 5.085073;

        // Combi terrain 2
        public readonly double CB2p1Latitude = 51.834871;
        public readonly double CB2p1Longitude = 5.08471;

        public readonly double CB2p2Latitude = 51.834871;
        public readonly double CB2p2Longitude = 5.085042;

        public readonly double CB2p3Latitude = 51.834436;
        public readonly double CB2p3Longitude = 5.084975;

        public readonly double CB2p4Latitude = 51.834470;
        public readonly double CB2p4Longitude = 5.084643;

        //Binnenkomst Vans/ Entry(New) Vans
        public readonly double EVp1Latitude = 51.833505;
        public readonly double EVp1Longitude = 5.085585;

        public readonly double EVp2Latitude = 51.833556;
        public readonly double EVp2Longitude = 5.085131;

        public readonly double EVp3Latitude = 51.833808;
        public readonly double EVp3Longitude = 5.085249;

        public readonly double EVp4Latitude = 51.833706;
        public readonly double EVp4Longitude = 5.086168;

        public readonly double EVp5Latitude = 51.833611;
        public readonly double EVp5Longitude = 5.086134;

        public readonly double EVp6Latitude = 51.833655;
        public readonly double EVp6Longitude = 5.085664;

        //Entry/New Trucks
        public readonly double ETp1Latitude = 51.833822;
        public readonly double ETp1Longitude = 5.086079;

        public readonly double ETp2Latitude = 51.833891;
        public readonly double ETp2Longitude = 5.085233;

        public readonly double ETp3Latitude = 51.834226;
        public readonly double ETp3Longitude = 5.085321;

        public readonly double ETp4Latitude = 51.834147;
        public readonly double ETp4Longitude = 5.086206;

        //Trekkers/Pull Trucks
        public readonly double TRp1Latitude = 51.834436;
        public readonly double TRp1Longitude = 5.084975;

        public readonly double TRp2Latitude = 51.834871;
        public readonly double TRp2Longitude = 5.085042;

        public readonly double TRp3Latitude = 51.834326;
        public readonly double TRp3Longitude = 5.089964;

        public readonly double TRp4Latitude = 51.833691;
        public readonly double TRp4Longitude = 5.089889;

        public readonly double TRp5Latitude = 51.833824;
        public readonly double TRp5Longitude = 5.088575;

        public readonly double TRp6Latitude = 51.834299;
        public readonly double TRp6Longitude = 5.088736;

        public readonly double TRp7Latitude = 51.834542;
        public readonly double TRp7Longitude = 5.086909;

        public readonly double TRp8Latitude = 51.834214;
        public readonly double TRp8Longitude = 5.086772;

        //TP1919/Truck Parts 1919
        public readonly double TRPp1Latitude = 51.833598;
        public readonly double TRPp1Longitude = 5.088073;

        public readonly double TRPp2Latitude = 51.833698;
        public readonly double TRPp2Longitude = 5.087352;

        public readonly double TRPp3Latitude = 51.833913;
        public readonly double TRPp3Longitude = 5.087424;

        public readonly double TRPp4Latitude = 51.833810;
        public readonly double TRPp4Longitude = 5.088166;

        //Deliver Vans
        public readonly double DVp1Latitude = 51.833592;
        public readonly double DVp1Longitude = 5.089915;

        public readonly double DVp2Latitude = 51.833599;
        public readonly double DVp2Longitude = 5.089596;

        public readonly double DVp3Latitude = 51.833322;
        public readonly double DVp3Longitude = 5.089502;

        public readonly double DVp4Latitude = 51.833295;
        public readonly double DVp4Longitude = 5.089814;

        //Combi Terrain 3
        public readonly double CB3p1Latitude = 51.833363;
        public readonly double CB3p1Longitude = 5.088191;

        public readonly double CB3p2Latitude = 51.833239;
        public readonly double CB3p2Longitude = 5.088118;

        public readonly double CB3p3Latitude = 51.833046;
        public readonly double CB3p3Longitude = 5.089381;

        public readonly double CB3p4Latitude = 51.833194;
        public readonly double CB3p4Longitude = 5.089459;

        //Bakwagens/ Bin Truks Terrain 1
        public readonly double BT1p1Latitude = 51.833046;
        public readonly double BT1p1Longitude = 5.089381;

        public readonly double BT1p2Latitude = 51.833194;
        public readonly double BT1p2Longitude = 5.089459;

        public readonly double BT1p3Latitude = 51.833146;
        public readonly double BT1p3Longitude = 5.089834;

        public readonly double BT1p4Latitude = 51.832799;
        public readonly double BT1p4Longitude = 5.089700;

        public readonly double BT1p5Latitude = 51.832809;
        public readonly double BT1p5Longitude = 5.089584;

        public readonly double BT1p6Latitude = 51.833012;
        public readonly double BT1p6Longitude = 5.089656;

        //Bin Trucks Area 2 
        public readonly double BT2p1Latitude = 51.833046;
        public readonly double BT2p1Longitude = 5.089381;

        public readonly double BT2p2Latitude = 51.833239;
        public readonly double BT2p2Longitude = 5.088118;

        public readonly double BT2p3Latitude = 51.832571;
        public readonly double BT2p3Longitude = 5.087972;

        public readonly double BT2p4Latitude = 51.832445;
        public readonly double BT2p4Longitude = 5.089339;

        //Bin Trucks Area 3
        public readonly double BT3p1Latitude = 51.833478;
        public readonly double BT3p1Longitude = 5.087096;

        public readonly double BT3p2Latitude = 51.833003;
        public readonly double BT3p2Longitude = 5.086903;

        public readonly double BT3p3Latitude = 51.832976;
        public readonly double BT3p3Longitude = 5.087019;

        public readonly double BT3p4Latitude = 51.832562;
        public readonly double BT3p4Longitude = 5.086886;

        public readonly double BT3p5Latitude = 51.832473;
        public readonly double BT3p5Longitude = 5.087810;

        public readonly double BT3p6Latitude = 51.833389;
        public readonly double BT3p6Longitude = 5.088017;

        //Kippers/Tippers Area 2
        public readonly double TP2p1Latitude = 51.832571;
        public readonly double TP2p1Longitude = 5.087972;

        public readonly double TP2p2Latitude = 51.832445;
        public readonly double TP2p2Longitude = 5.089339;

        public readonly double TP2p3Latitude = 51.832332;
        public readonly double TP2p3Longitude = 5.089309;

        public readonly double TP2p4Latitude = 51.832459;
        public readonly double TP2p4Longitude = 5.087923;

        //Combi Area 4 
        public readonly double CB4p1Latitude = 51.832913;
        public readonly double CB4p1Longitude = 5.086817;

        public readonly double CB4p2Latitude = 51.832339;
        public readonly double CB4p2Longitude = 5.086651;

        public readonly double CB4p3Latitude = 51.832264;
        public readonly double CB4p3Longitude = 5.087867;

        public readonly double CB4p4Latitude = 51.832397;
        public readonly double CB4p4Longitude = 5.087901;

        public readonly double CB4p5Latitude = 51.832541;
        public readonly double CB4p5Longitude = 5.086808;

        public readonly double CB4p6Latitude = 51.832906;
        public readonly double CB4p6Longitude = 5.086883;

        //Tippers Area 3
        public readonly double TP3p1Latitude = 51.832264;
        public readonly double TP3p1Longitude = 5.087867;

        public readonly double TP3p2Latitude = 51.832397;
        public readonly double TP3p2Longitude = 5.087901;

        public readonly double TP3p3Latitude = 51.832326;
        public readonly double TP3p3Longitude = 5.088618;

        public readonly double TP3p4Latitude = 51.832201;
        public readonly double TP3p4Longitude = 5.088596;

        // Mixer Trucks
        public readonly double MTp1Latitude = 51.832326;
        public readonly double MTp1Longitude = 5.088618;

        public readonly double MTp2Latitude = 51.832201;
        public readonly double MTp2Longitude = 5.088596;

        public readonly double MTp3Latitude = 51.832161;
        public readonly double MTp3Longitude = 5.089349;

        public readonly double MTp4Latitude = 51.832267;
        public readonly double MTp4Longitude = 5.089388;
    }
}
