using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DingoSystem;
using System.Net.NetworkInformation;
using System;

public static class DingoDatabase
{
    public static DingoID AgentBingo { get; } = new DingoID(0, "Agent Bingo", "Physical", "Agent Bingo is a skilled martial artist.", 2000, 84, 78, 100, "image1", 2000, 0, 50, 1);
    public static DingoID BingoStar { get; } = new DingoID(1, "BingoStar", "Light", "BingoStar flies around the sky in search of powerful opponents.", 200, 84, 78, 100, "star", 200, 0, 50, 1);
    public static DingoID Bean { get; } = new DingoID(2, "Bean", "Grass", "Bean has a grudge against certain people.", 200, 84, 78, 100, "bean", 200, 0, 50, 1);
    public static DingoID Bird { get; } = new DingoID(3, "Bird", "Wind", "Bird is the word.", 200, 90, 58, 100, "bird", 200, 0, 50, 1);
    public static DingoID Pinkthing { get; } = new DingoID(4, "Pinkthing", "Abnormal", "Pink guy.", 200, 22, 156, 100, "pinkthing", 200, 0, 50, 1);
    public static DingoID Waterdino { get; } = new DingoID(5, "Waterdino", "Water", "Loch Ness monster.", 200, 122, 46, 100, "waterdino", 200, 0, 50, 1);
    public static DingoID Weirdtongue { get; } = new DingoID(6, "Weirdtongue", "Fire", "Fire Loch Ness monster.", 200, 122, 46, 100, "weirdtongue", 200, 0, 50, 1);
    public static DingoID Magicfishy { get; } = new DingoID(7, "Magicfishy", "Dark", "Magicfishy roams the night with mysterious powers.", 200, 84, 78, 100, "Magicfishy", 200, 0, 50, 1);
    public static DingoID Marshmellow { get; } = new DingoID(8, "Marshmellow", "Fire", "Marshmellow is sweet and fluffy, spreading joy wherever it goes.", 200, 84, 78, 100, "marshmellow", 200, 0, 50, 1);
    public static DingoID Robbersnail { get; } = new DingoID(9, "Robbersnail", "Dark", "Robbersnail moves slowly but strikes with surprising speed and cunning.", 200, 84, 78, 100, "robbersnail", 200, 0, 50, 1);
    public static DingoID Rock { get; } = new DingoID(10, "Rock", "Physical", "Rock is sturdy and unyielding, standing firm against all attacks.", 200, 84, 78, 100, "rock", 200, 0, 50, 1);
    public static DingoID Seed { get; } = new DingoID(11, "Seed", "Grass", "Seed nurtures life, spreading seeds of hope across the land.", 200, 84, 78, 100, "seed", 200, 0, 50, 1);
    public static DingoID Buggy { get; } = new DingoID(12, "Buggy", "Grass", "Buggy scurries around with its many legs, always searching for new adventures.", 200, 70, 60, 100, "buggy", 200, 0, 50, 1);
    public static DingoID DancingPlant { get; } = new DingoID(13, "DancingPlant", "Grass", "DancingPlant sways gently in the breeze, spreading seeds of joy wherever it goes.", 200, 60, 70, 100, "dancingplant", 200, 0, 50, 1);
    public static DingoID Shingy { get; } = new DingoID(14, "Shingy", "Dark", "Shingy reflects the light with its metallic body, dazzling opponents in battle.", 200, 80, 75, 100, "shingy", 200, 0, 50, 1);
    public static DingoID SadCloud { get; } = new DingoID(15, "SadCloud", "Wind", "SadCloud drifts through the sky, leaving raindrops of melancholy in its wake.", 200, 50, 65, 100, "sadcloud", 200, 0, 50, 1);
    public static DingoID Worm { get; } = new DingoID(16, "Worm", "Physical", "Worm burrows through the earth, emerging to surprise unsuspecting foes.", 200, 55, 50, 100, "worm", 200, 0, 50, 1);
    public static DingoID Pebble { get; } = new DingoID(17, "Pebble", "Dark", "Pebble may seem small, but its tough exterior can withstand powerful attacks.", 200, 40, 90, 100, "pebble", 200, 0, 50, 1);
    public static DingoID Ghost { get; } = new DingoID(18, "Ghost", "Dark", "Ghost haunts the shadows, striking fear into the hearts of its enemies.", 200, 70, 90, 100, "ghost", 200, 0, 50, 1);
    public static DingoID Crick { get; } = new DingoID(19, "Crick", "Grass", "Crick is a Grass type insect.", 200, 80, 60, 120, "crick", 200, 0, 50, 1);
    public static DingoID Firefly { get; } = new DingoID(20, "Firefly", "Fire", "Firefly is a fire type insect.", 200, 90, 50, 110, "firefly", 200, 0, 50, 1);
    public static DingoID Freddy { get; } = new DingoID(21, "Freddy", "Abnormal", "Freddy is an abnormal type Dingo.", 200, 70, 80, 100, "freddy", 200, 0, 50, 1);
    public static DingoID Fried { get; } = new DingoID(22, "Fried", "Abnormal", "Fried is another abnormal type Dingo.", 200, 75, 75, 95, "fried", 200, 0, 50, 1);
    public static DingoID Icemunchkin { get; } = new DingoID(23, "Icemunchkin", "Ice", "Icemunchkin is an ice type Dingo.", 200, 85, 65, 105, "icemunchkin", 200, 0, 50, 1);
    public static DingoID Octi { get; } = new DingoID(24, "Octi", "Water", "Octi is a water type octopus Dingo.", 200, 95, 55, 115, "octi", 200, 0, 50, 1);
    public static DingoID Bulb { get; } = new DingoID(25, "Bulb", "Lightning", "Bulb emits a sinister glow, harnessing the power of darkness and electricity.", 250, 50, 80, 120, "bulb", 250, 0, 60, 1);
    public static DingoID Tanktop { get; } = new DingoID(26, "Tanktop", "Abnormal", "Tanktop is an abnormal type Dingo with a tough exterior.", 200, 100, 90, 85, "tanktop", 200, 0, 50, 1);
    public static DingoID TrustFundBaby { get; } = new DingoID(27, "TrustFundBaby", "Finance", "TrustFundBaby is a finance type Dingo.", 200, 80, 70, 95, "trustfundbaby", 200, 0, 50, 1);
    public static DingoID Plant { get; } = new DingoID(28, "Plant", "Grass", "Plant is a Grass type Dingo.", 200, 75, 80, 90, "plant", 200, 0, 50, 1);
    public static DingoID Waterslime { get; } = new DingoID(29, "Waterslime", "Water", "Waterslime is a water type Dingo.", 200, 82, 81, 90, "waterslime", 200, 0, 50, 1);
    public static DingoID Terrortentacle { get; } = new DingoID(30, "Terrortentacle", "Dark", "Terrortentacle is a ghost type Dingo.", 200, 110, 70, 80, "terrortentacle", 200, 0, 50, 1);
    public static DingoID Ducky { get; } = new DingoID(31, "Ducky", "Wind", "Ducky gracefully glides through the air with its feather-like wings, harnessing the power of the wind.", 200, 75, 85, 90, "ducky", 200, 0, 50, 1);
    public static DingoID Forqwa { get; } = new DingoID(32, "Forqwa", "Physical", "Forqwa is a sturdy and muscular Dingo, known for its powerful physical attacks.", 200, 95, 80, 75, "forqwa", 200, 0, 50, 1);
    public static DingoID Mustache { get; } = new DingoID(33, "Mustache", "Abnormal", "Mustache is a mysterious Dingo with an unusual appearance, its mustache-like tendrils hiding its true intentions.", 200, 70, 70, 100, "mustache", 200, 0, 50, 1);
    public static DingoID Flame { get; } = new DingoID(34, "Flame", "Fire", "Flame is a flamer", 200, 70, 70, 100, "flame", 200, 0, 50, 1);
    public static DingoID Doggofire { get; } = new DingoID(35, "Doggo Fire", "Fire", "Doggo Fire is the fire type", 200, 70, 70, 100, "doggofire", 200, 0, 50, 1);
    public static DingoID Doggoelectric { get; } = new DingoID(36, "Doggo Electric", "Lightning", "Doggo electric is the electric type", 200, 70, 70, 100, "doggoelectric", 200, 0, 50, 1);
    public static DingoID Doggoice { get; } = new DingoID(37, "Doggo Ice", "Ice", "Doggo Ice is the ice type", 200, 70, 70, 100, "doggoice", 200, 0, 50, 1);
    public static DingoID Doggograss { get; } = new DingoID(38, "Doggo Grass", "Grass", "Doggo Grass is the grass type", 200, 70, 70, 100, "doggograss", 200, 0, 50, 1);
    public static DingoID Doggowater { get; } = new DingoID(39, "Doggo Water", "Water", "Doggo Water is the water type", 200, 70, 70, 100, "doggowater", 200, 0, 50, 1);
    public static DingoID BingoStar2 { get; } = new DingoID(1000, "BingoStar", "Light", "BingoStar flies around the sky in search of powerful opponents.", 200, 1, 1, 1, "star", 200, 0, 50, 1);
    public static DingoID Marshmellow2 { get; } = new DingoID(1001, "Marshmellow", "Fire", "Marshmellow is sweet and fluffy, spreading joy wherever it goes.", 200, 1, 1, 1, "marshmellow", 200, 0, 50, 1);
    public static EnvironmentEffect Rain { get; } = new EnvironmentEffect(0, 3, "Rain");
    public static StatusEffect Goo { get; } = new StatusEffect(0, 3, "Goo");


    // Add more base Dingos as needed
    static DingoDatabase()
    {

        AgentBingo.AddMove(new DingoMove(0, "Quick Jab", "Physical", 0, 30, 100, null, null, "A fast and precise punch aimed at the opponent's guard."));
        AgentBingo.AddMove(new DingoMove(1, "Heavy Kick", "Physical", 0, 40, 95, null, null, "A powerful roundhouse kick meant to knock the opponent back."));
        AgentBingo.AddMove(new DingoMove(2, "Leg Sweep", "Physical", 0, 35, 100, null, null, "A low sweeping kick that may cause the opponent to stumble."));
        AgentBingo.AddMove(new DingoMove(3, "Uppercut", "Physical", 0, 50, 90, null, null, "A strong upward punch that can launch lighter foes."));
        AgentBingo.AddMove(new DingoMove(4, "Squirt Gun", "Water", 0, 10, 100, null, null, "Fires a short-range taser that deals damage and may paralyze."));
        AgentBingo.AddMove(new DingoMove(5, "Equip Knife", "Physical", 0, 0, 100, null, null, "Uses a hook to reposition quickly, boosting evasion and speed."));
        AgentBingo.AddMove(new DingoMove(6, "Baton Strike", "Physical", 0, 60, 95, null, null, "Strikes the target with a collapsible baton for solid damage."));
        AgentBingo.AddMove(new DingoMove(7, "Flamethrower", "Fire", 0, 70, 90, null, null, "Disables enemy tech and deals moderate damage to mechanical foes."));
        AgentBingo.AddMove(new DingoMove(8, "Tactical Shield Bash", "Physical", 0, 65, 100, null, null, "Uses a compact shield to bash the opponent, raising defense."));
        AgentBingo.AddMove(new DingoMove(9, "Run Over", "Physical", 0, 90, 85, null, null, "Plants and detonates a small charge for heavy damage."));


        BingoStar.AddMove(new DingoMove(0, "Shooting Star", "Fire", 0, 35, 90, null, null, "Summons the power of the stars to strike the opponent."));
        BingoStar.AddMove(new DingoMove(1, "Luminous Burst", "Light", 0, 40, 100, null, null, "A burst of radiant light that dazzles the opponent."));
        BingoStar.AddMove(new DingoMove(2, "Eclipse", "Light", 0, 0, 100, null, null, "Conjures a temporary eclipse that shrouds the battlefield, increases light attack moves."));
        BingoStar.AddMove(new DingoMove(3, "Cosmic Shield", "Light", 0, 0, 100, null, null, "Creates a protective shield using cosmic energy, raising defense sharply."));
        BingoStar.AddMove(new DingoMove(4, "Starfall", "Light", 0, 55, 95, null, null, "Calls upon the stars to rain down upon the opponent."));
        BingoStar.AddMove(new DingoMove(5, "Stellar Dance", "Light", 0, 0, 100, null, null, "Dances gracefully among the stars, increasing attack."));
        BingoStar.AddMove(new DingoMove(6, "Galactic Blast", "Light", 0, 70, 100, null, null, "Unleashes a powerful blast of energy from distant galaxies."));
        BingoStar.AddMove(new DingoMove(7, "Celestial Flare", "Fire", 0, 85, 100, null, null, "The user summons a burst of stellar energy from the heavens, showering down on opponents with intense light, dealing damage."));
        BingoStar.AddMove(new DingoMove(8, "Nebula Nova", "Light", 0, 100, 90, null, null, "Creates a swirling nebula of energy around the opponent, dealing damage."));
        BingoStar.AddMove(new DingoMove(9, "Supernova Surge", "Light", 0, 120, 80, null, null, "Releases a massive surge of energy reminiscent of a dying star."));

        Bean.AddMove(new DingoMove(0, "Vine Slash", "Grass", 0, 30, 100, null, null, "The target is struck with slender, whiplike vines to inflict damage."));
        Bean.AddMove(new DingoMove(1, "Bean Throw", "Grass", 0, 40, 90, null, null, "The user picks up and throws a bean at the target to attack."));
        Bean.AddMove(new DingoMove(2, "Quake", "Physical", 0, 40, 100, null, null, "Causes a powerful earthquake that damages Dingos on the field."));
        Bean.AddMove(new DingoMove(9, "Photosynthesis", "Grass", 0, 0, 100, null, null, "Absorbs sunlight to restore health."));
        Bean.AddMove(new DingoMove(4, "Bean Bomb", "Grass", 0, 50, 100, null, null, "The user slams a barrage of beans down on the target from above."));
        Bean.AddMove(new DingoMove(5, "Harmonic Growth", "Grass", 0, 0, 100, null, null, "The user's body grows all at once, raising the Attack and Defense stats."));
        Bean.AddMove(new DingoMove(3, "Verdant Whirlwind", "Grass", 0, 30, 85, null, null, "The user creates a swirling vortex of leaves and wind, with a chance to hit the opponent multiple times."));
        Bean.AddMove(new DingoMove(6, "Leaf Slash", "Grass", 0, 70, 100, null, null, "The user handles a sharp leaf like a sword and attacks by cutting its target."));
        Bean.AddMove(new DingoMove(7, "Wild Growth", "Grass", 0, 85, 100, null, null, "The user causes wild vegetation to rapidly grow and entangle the opponent, dealing damage and potentially trapping them for a two turns."));
        Bean.AddMove(new DingoMove(8, "Nature's Fury", "Grass", 0, 110, 85, null, null, "Calls upon the forces of nature to unleash a devastating storm."));

        Bird.AddMove(new DingoMove(0, "Air Strike", "Wind", 0, 30, 100, null, null, "A burst of high-speed air strikes the target with speed."));
        Bird.AddMove(new DingoMove(1, "Shadow Bind", "Dark", 0, 35, 100, null, null, "The user ensnares the opponent in tendrils of darkness, dealing damage."));
        Bird.AddMove(new DingoMove(2, "Wind Whisk", "Wind", 0, 40, 90, null, null, "The user releases a powerful blast of wind at the opponent, dealing damage."));
        Bird.AddMove(new DingoMove(3, "Build Nest", "Wind", 0, 0, 100, null, null, "The user starts building a sturdy nest, increasing its defense and evasion."));
        Bird.AddMove(new DingoMove(5, "Sky Dive", "Wind", 0, 55, 95, null, null, "Plummets from the sky with incredible speed to strike the opponent."));
        Bird.AddMove(new DingoMove(4, "Whirlwind Dance", "Wind", 0, 0, 100, null, null, "Elegantly dances through the air, sharply reducing the opponent's attack."));
        Bird.AddMove(new DingoMove(6, "Aero Slicer", "Wind", 0, 30, 85, null, null, "The user releases sharp blades of compressed air at the opponent, with a chance to hit the opponent multiple times."));
        Bird.AddMove(new DingoMove(7, "Cloud Burst", "Wind", 0, 85, 95, null, null, "Gathers moisture from the air to create a powerful burst of cloud energy."));
        Bird.AddMove(new DingoMove(8, "Umbral Strike", "Dark", 0, 90, 95, null, null, "The user strikes the opponent with a dark, shadowy force, dealing damage"));
        Bird.AddMove(new DingoMove(9, "Typhoon Fury", "Wind", 0, 120, 80, null, null, "The user summons a raging typhoon that strikes the opponent with immense force."));

        Pinkthing.AddMove(new DingoMove(0, "Pink Blast", "Abnormal", 0, 35, 100, null, null, "A mysterious pink energy is fired at the target, dealing damage."));
        Pinkthing.AddMove(new DingoMove(1, "Volt Surge", "Lightning", 0, 40, 100, null, null, "The user channels electricity through the ground, causing a surge of electrical energy to erupt beneath the opponent, dealing damage."));
        Pinkthing.AddMove(new DingoMove(2, "Stupid Pink", "Abnormal", 0, 0, 100, null, null, "The user emits a bizarre and confusing pink aura, causing the opponent to become disoriented."));
        Pinkthing.AddMove(new DingoMove(3, "Ionize", "Lightning", 0, 0, 100, null, null, "The user charges the air with ions, creating a volatile electrical field that increases its attack."));
        Pinkthing.AddMove(new DingoMove(4, "Jolt Cannon", "Lightning", 0, 50, 100, null, null, "The user fires a concentrated bolt of lightning at the opponent, dealing damage."));
        Pinkthing.AddMove(new DingoMove(5, "Static Field", "Lightning", 0, 0, 90, null, null, "The user creates a static field that electrifies the battlefield, causing the opponent to be unable to attack for two turns."));
        Pinkthing.AddMove(new DingoMove(6, "Thunderbolt Barrage", "Lightning", 0, 30, 85, null, null, "The user unleashes a rapid barrage of lightning bolts at the opponent, with a chance to hit the opponent multiple times."));
        Pinkthing.AddMove(new DingoMove(7, "Thunderstorm Fury", "Lightning", 0, 70, 100, null, null, "The user summons a fierce thunderstorm above the battlefield, raining down bolts of lightning, dealing damage."));
        Pinkthing.AddMove(new DingoMove(8, "Charge Blitz", "Lightning", 0, 85, 100, null, null, "The user charges forward with electrifying speed, crashing into the opponent with a powerful tackle that deals damage and increases the user's Attack stat."));
        Pinkthing.AddMove(new DingoMove(9, "Plasma Wave", "Lightning", 0, 100, 90, null, null, "The user releases a wave of superheated plasma, dealing damage."));

        Waterdino.AddMove(new DingoMove(0, "Water Cannon", "Water", 0, 30, 100, null, null, "The target is blasted with a forceful shot of water."));
        Waterdino.AddMove(new DingoMove(1, "Hydro Slice", "Water", 0, 40, 90, null, null, ""));
        Waterdino.AddMove(new DingoMove(2, "Aqua Shield", "Water", 0, 0, 100, null, null, "Forms a protective shield of water to reduce incoming damage."));
        Waterdino.AddMove(new DingoMove(3, "Look Sad", "Water", 0, 0, 100, "looksad", null, "The user gives a pitiful look, causing the opponent's morale to plummet, lowering the oponent's defense."));
        Waterdino.AddMove(new DingoMove(4, "Nightfall Strike", "Dark", 0, 60, 100, null, null, "The user strikes the opponent with a dark energy infused attack, dealing damage."));
        Waterdino.AddMove(new DingoMove(5, "Hydro Beam", "Water", 0, 60, 100, null, null, "The user unleashes a concentrated beam of pressurized water at the opponent, dealing damage"));
        Waterdino.AddMove(new DingoMove(6, "Torrential Downpour", "Water", 0, 0, 100, null, null, "The user summons a torrential downpour that drenches the battlefield, boosting its attack."));
        Waterdino.AddMove(new DingoMove(7, "Tidal Wave", "Water", 0, 85, 100, null, null, "Creates a massive wave that crashes down on the opponent."));
        Waterdino.AddMove(new DingoMove(8, "Umbral Strike", "Dark", 0, 90, 95, null, null, "The user strikes the opponent with a dark, shadowy force, dealing damage"));
        Waterdino.AddMove(new DingoMove(9, "Hydro Vortex", "Water", 0, 100, 90, null, null, "The user creates a swirling vortex of water that engulfs the opponent, dealing damage"));

        Weirdtongue.AddMove(new DingoMove(0, "Scorching Fire", "Fire", 0, 30, 100, null, null, "The target is scorched with an intense blast of fire."));
        Weirdtongue.AddMove(new DingoMove(1, "Sinister Drain", "Dark", 0, 35, 100, null, null, "The user drains the opponent's energy through dark means, dealing damage."));
        Weirdtongue.AddMove(new DingoMove(2, "Dusk Shroud", "Dark", 0, 0, 100, null, null, "The user cloaks itself in a shroud of darkness, sharply raising its defense."));
        Weirdtongue.AddMove(new DingoMove(3, "Weird Lick", "Abnormal", 0, 30, 90, null, null, "Target got licked."));
        Weirdtongue.AddMove(new DingoMove(4, "Blazing Rampart", "Fire", 0, 0, 100, null, null, "The user creates a wall of intense flames that shields it from all attacks for two turns."));
        Weirdtongue.AddMove(new DingoMove(5, "Cursed Fangs", "Dark", 0, 55, 100, null, null, "The user bites down with fangs imbued with dark energy, dealing damage"));
        Weirdtongue.AddMove(new DingoMove(6, "Incendiary Impact", "Fire", 0, 70, 100, null, null, "The user charges at the opponent with explosive force, dealing damage."));
        Weirdtongue.AddMove(new DingoMove(7, "Magma Surge", "Fire", 0, 85, 95, null, null, "Causes a surge of molten magma to erupt from beneath the opponent, dealing damage."));
        Weirdtongue.AddMove(new DingoMove(8, "Umbral Strike", "Dark", 0, 90, 95, null, null, "The user strikes the opponent with a dark, shadowy force, dealing damage."));
        Weirdtongue.AddMove(new DingoMove(9, "Blazing Meteor", "Fire", 0, 120, 80, null, null, "The user summons a meteor made of pure fire from the sky to crash into the opponent, dealing damage."));

        Magicfishy.AddMove(new DingoMove(0, "Water Gun", "Water", 0, 40, 100, null, null, "The target is blasted with a forceful shot of water."));
        Magicfishy.AddMove(new DingoMove(1, "Bubble", "Water", 0, 40, 100, null, null, "A spray of countless bubbles is jetted at the opposing Pokémon. This may also lower their Speed stats."));
        Magicfishy.AddMove(new DingoMove(2, "Octazooka", "Water", 0, 65, 85, null, null, "The user attacks by spraying ink in the target's face or eyes. This may also lower the target's accuracy."));
        Magicfishy.AddMove(new DingoMove(3, "Aqua Jet", "Water", 0, 40, 100, null, null, "The user lunges at the target at a speed that makes it almost invisible. This move always goes first."));
        Magicfishy.AddMove(new DingoMove(4, "Water Pulse", "Water", 0, 60, 100, null, null, "The user attacks the target with a pulsing blast of water. This may also confuse the target."));
        Magicfishy.AddMove(new DingoMove(5, "Bubble Beam", "Water", 0, 65, 100, null, null, "A spray of bubbles is forcefully ejected at the target. This may also lower its Speed stat."));
        Magicfishy.AddMove(new DingoMove(6, "Hydro Pump", "Water", 0, 110, 80, null, null, "The target is blasted by a huge volume of water launched under great pressure."));
        Magicfishy.AddMove(new DingoMove(7, "Surf", "Water", 0, 90, 100, null, null, "The user attacks everything around it by swamping its surroundings with a giant wave."));
        Magicfishy.AddMove(new DingoMove(8, "Waterfall", "Water", 0, 80, 100, null, null, "The user charges at the target and may make it flinch."));
        Magicfishy.AddMove(new DingoMove(9, "Liquidation", "Water", 0, 85, 100, null, null, "The user slams into the target using a full-force blast of water. This may also lower the target's Defense stat."));

        Marshmellow.AddMove(new DingoMove(0, "Sugar Slam", "Abnormal", 0, 35, 100, null, null, "The user slams into the opponent with sweet force, causing damage."));
        Marshmellow.AddMove(new DingoMove(1, "Sweet Shield", "Abnormal", 0, 0, 100, null, null, "The user forms a shield of sweetness, raising defense sharply."));
        Marshmellow.AddMove(new DingoMove(2, "Toasted Toss", "Fire", 0, 20, 100, null, null, "The user throws a toasted marshmallow at the opponent, dealing damage."));
        Marshmellow.AddMove(new DingoMove(3, "Gooey Glare", "Abnormal", 0, 0, 100, null, null, "The user gives the opponent a gooey glare, lowering their attack and defense."));
        Marshmellow.AddMove(new DingoMove(4, "Mallow Munch", "Abnormal", 0, 0, 100, null, null, "The user munches on a marshmallow, restoring health."));
        Marshmellow.AddMove(new DingoMove(5, "Toasty Tumble", "Fire", 0, 50, 100, null, null, "The user rolls a flaming marshmallow at the opponent, dealing damage."));
        Marshmellow.AddMove(new DingoMove(6, "Marshmallow Melt", "Fire", 0, 65, 95, null, null, "The user heats up its body and charges forth at the opponent, dealing damage."));
        Marshmellow.AddMove(new DingoMove(7, "Sugar Spin", "Abnormal", 0, 85, 100, null, null, "The user spins around in a sweet whirlwind, striking the opponent."));
        Marshmellow.AddMove(new DingoMove(8, "Roasty Beam", "Fire", 0, 95, 95, null, null, "The user fires a beam of searing marshmallow heat at the opponent, dealing damage."));
        Marshmellow.AddMove(new DingoMove(9, "Squishy Frenzy", "Abnormal", 0, 110, 85, "goo", null, "The user goes into a squishy frenzy, causing massive damage to the opponent."));

        Robbersnail.AddMove(new DingoMove(0, "Sneaky Shell", "Dark", 0, 0, 100, null, null, "The user retreats into its shell, becoming invisible and avoiding attacks."));
        Robbersnail.AddMove(new DingoMove(1, "Venomous Trail", "Abnormal", 0, 70, 95, null, null, "The user leaves a trail of toxic slime that poisons the opponent upon contact."));
        Robbersnail.AddMove(new DingoMove(2, "Sneaky Strike", "Dark", 0, 80, 100, null, null, "The user emerges from hiding to deliver a swift and unexpected strike to the opponent."));
        Robbersnail.AddMove(new DingoMove(3, "Shell Shock", "Abnormal", 0, 90, 90, null, null, "The user strikes the opponent with its shell, causing damage."));
        Robbersnail.AddMove(new DingoMove(4, "Slime Shot", "Abnormal", 0, 60, 100, null, null, "The user shoots a blob of sticky slime at the opponent, causing damage."));
        Robbersnail.AddMove(new DingoMove(5, "Invisible Inc", "Dark", 0, 0, 100, null, null, "The user cloaks itself in darkness, becoming invisible to the opponent."));
        Robbersnail.AddMove(new DingoMove(6, "Toxic Tackle", "Abnormal", 0, 85, 90, null, null, "The user tackles the opponent with a toxic force, causing damage and poisoning."));
        Robbersnail.AddMove(new DingoMove(7, "Quick Retreat", "Abnormal", 0, 0, 100, null, null, "The user retreats into its shell, avoiding attacks and gaining speed."));
        Robbersnail.AddMove(new DingoMove(8, "Sneaky Snail", "Dark", 0, 0, 100, null, null, "The user sneaks up on the opponent with stealth, surprising them."));
        Robbersnail.AddMove(new DingoMove(9, "Slimy Strike", "Abnormal", 0, 75, 100, null, null, "The user strikes the opponent with a slimy attack, causing damage."));

        Rock.AddMove(new DingoMove(0, "Rock Throw", "Physical", 0, 50, 90, null, null, "The user picks up and throws a small rock at the target to attack."));
        Rock.AddMove(new DingoMove(1, "Tectonic Slam", "Physical", 0, 120, 70, null, null, "Slams the ground with tremendous force, causing a massive shockwave."));
        Rock.AddMove(new DingoMove(2, "Crystal Barrier", "Physical", 0, 0, 100, null, null, "Forms a barrier of hardened crystal that reflects incoming attacks."));
        Rock.AddMove(new DingoMove(3, "Rock Slide", "Physical", 0, 75, 90, null, null, "Large boulders are hurled at the opposing Pokémon to inflict damage."));
        Rock.AddMove(new DingoMove(4, "Quicksand Quake", "Physical", 0, 70, 95, null, null, "Triggers a sudden quake that turns the ground into quicksand, trapping the opponent."));
        Rock.AddMove(new DingoMove(5, "Hard Shell", "Physical", 0, 0, 100, null, null, "The user hardens its body like rock, sharply raising its defense."));
        Rock.AddMove(new DingoMove(6, "Rock Throw", "Physical", 0, 50, 90, null, null, "The user picks up and throws a small rock at the target to attack."));
        Rock.AddMove(new DingoMove(7, "Rock Polish", "Physical", 0, 0, 100, null, null, "The user polishes its body to reduce drag, raising its Speed stat."));
        Rock.AddMove(new DingoMove(8, "Rock Smash", "Physical", 0, 40, 100, null, null, "The user smashes into the target with its whole body, causing damage."));
        Rock.AddMove(new DingoMove(9, "Molten Fissure", "Fire", 0, 100, 80, null, null, "Creates a fissure in the ground, releasing molten rock to scorch the opponent."));

        Seed.AddMove(new DingoMove(0, "Seed Bomb", "Grass", 0, 80, 100, null, null, "The user slams a barrage of hard-shelled seeds down on the target from above."));
        Seed.AddMove(new DingoMove(1, "Growth", "Grass", 0, 0, 100, null, null, "The user's body grows all at once, raising the Attack and Sp. Atk stats."));
        Seed.AddMove(new DingoMove(2, "Leaf Blade", "Grass", 0, 90, 100, null, null, "The user handles a sharp leaf like a sword and attacks by cutting its target."));
        Seed.AddMove(new DingoMove(3, "Synthesis", "Grass", 0, 0, 100, null, null, "The user restores its own HP. The amount of HP regained varies with the weather."));
        Seed.AddMove(new DingoMove(4, "Solar Beam", "Grass", 0, 120, 100, null, null, "Gathers solar energy and unleashes it in a powerful beam."));
        Seed.AddMove(new DingoMove(5, "Pollen Blast", "Grass", 0, 90, 95, null, null, "Launches a blast of pollen that inflicts damage and may induce sleep."));
        Seed.AddMove(new DingoMove(6, "Blossom Shield", "Grass", 0, 0, 100, null, null, "Surrounds itself with a shield of blossoms, raising defense and restoring health."));
        Seed.AddMove(new DingoMove(7, "Razor Leaf", "Grass", 0, 55, 95, null, null, "Sharp-edged leaves are launched to slash at the opposing Pokémon."));
        Seed.AddMove(new DingoMove(8, "Grass Knot", "Grass", 0, 0, 100, null, null, "The user snares the target with grass and trips it. The heavier the target, the greater the move's power."));
        Seed.AddMove(new DingoMove(9, "Magical Leaf", "Grass", 0, 60, 100, null, null, "The user scatters curious leaves that chase the target. This attack never misses."));

        Buggy.AddMove(new DingoMove(0, "Bug Swarm", "Grass", 0, 80, 100, null, null, "Summons a swarm of bugs to overwhelm the opponent."));
        Buggy.AddMove(new DingoMove(1, "Sticky Web", "Grass", 0, 0, 100, null, null, "Weaves a sticky web that slows down and traps the opponent."));
        Buggy.AddMove(new DingoMove(2, "Acid Spray", "Grass", 0, 70, 100, null, null, "Sprays a corrosive acid that melts through defenses."));
        Buggy.AddMove(new DingoMove(3, "Poison Sting", "Grass", 0, 15, 100, null, null, "The user stabs the target with a poisonous stinger. This may also poison the target."));
        Buggy.AddMove(new DingoMove(4, "Bug Bite", "Grass", 0, 60, 100, null, null, "The user bites the target. If the target is holding a Berry, the user eats it and gains its effect."));
        Buggy.AddMove(new DingoMove(5, "String Shot", "Grass", 0, 0, 95, null, null, "The target is bound with silk blown from the user's mouth, reducing the target's Speed stat."));
        Buggy.AddMove(new DingoMove(6, "Harden", "Grass", 0, 0, 100, null, null, "The user stiffens all the muscles in its body to raise its Defense stat."));
        Buggy.AddMove(new DingoMove(7, "X-Scissor", "Grass", 0, 80, 100, null, null, "The user slashes at the target by crossing its scythes or claws as if they were a pair of scissors."));
        Buggy.AddMove(new DingoMove(8, "Bug Buzz", "Grass", 0, 90, 100, null, null, "The user generates a damaging sound wave by vibration. This may also lower the target's Sp. Def stat."));
        Buggy.AddMove(new DingoMove(9, "Bug Bite", "Grass", 0, 60, 100, null, null, "The user bites the target. If the target is holding a Berry, the user eats it and gains its effect."));

        DancingPlant.AddMove(new DingoMove(0, "Petal Dance", "Grass", 0, 120, 100, null, null, "Dances gracefully while scattering sharp petals that damage the opponent."));
        DancingPlant.AddMove(new DingoMove(1, "Thorn Whip", "Grass", 0, 85, 95, null, null, "Whips the opponent with sharp thorns, causing damage and possible flinching."));
        DancingPlant.AddMove(new DingoMove(2, "Meadow Melody", "Grass", 0, 0, 100, null, null, "Sings a soothing melody that restores health and removes status conditions."));
        DancingPlant.AddMove(new DingoMove(3, "Sleep Powder", "Grass", 0, 0, 75, null, null, "The user scatters a big cloud of sleep-inducing dust around the target."));
        DancingPlant.AddMove(new DingoMove(4, "Magical Leaf", "Grass", 0, 60, 100, null, null, "The user scatters curious leaves that chase the target. This attack never misses."));
        DancingPlant.AddMove(new DingoMove(5, "Synthesis", "Grass", 0, 0, 100, null, null, "The user restores its own HP. The amount of HP regained varies with the weather."));
        DancingPlant.AddMove(new DingoMove(6, "Pollen Blast", "Grass", 0, 90, 95, null, null, "Launches a blast of pollen that inflicts damage and may induce sleep."));
        DancingPlant.AddMove(new DingoMove(7, "Leaf Blade", "Grass", 0, 90, 100, null, null, "The user handles a sharp leaf like a sword and attacks by cutting its target."));
        DancingPlant.AddMove(new DingoMove(8, "Growth", "Grass", 0, 0, 100, null, null, "The user's body grows all at once, raising the Attack and Sp. Atk stats."));
        DancingPlant.AddMove(new DingoMove(9, "Grassy Terrain", "Grass", 0, 0, 100, null, null, "The user covers the ground with grass and revitalizes all Pokémon on the ground for five turns."));

        Shingy.AddMove(new DingoMove(0, "Shadow Ball", "Dark", 0, 80, 100, null, null, "The user hurls a shadowy blob at the target. This may also lower the target's Special Defense stat."));
        Shingy.AddMove(new DingoMove(1, "Sucker Punch", "Dark", 0, 70, 100, null, null, "This move enables the user to attack first. This move fails if the target is not readying an attack."));
        Shingy.AddMove(new DingoMove(2, "Shadow Claw", "Dark", 0, 70, 100, null, null, "The user slashes with a sharp claw made from shadows. Critical hits land more easily."));
        Shingy.AddMove(new DingoMove(3, "Night Shade", "Dark", 0, 0, 100, null, null, "The user makes the target see a frightening mirage. It inflicts damage matching the user’s level."));
        Shingy.AddMove(new DingoMove(4, "Spirit Bomb", "Light", 0, 90, 100, null, null, "The user gathers spiritual energy and releases it at once. This may also lower the target's Special Defense stat."));
        Shingy.AddMove(new DingoMove(5, "Shadow Sneak", "Dark", 0, 40, 100, null, null, "The user extends its shadow and attacks the target from behind. This move always goes first."));
        Shingy.AddMove(new DingoMove(6, "Hex", "Dark", 0, 65, 100, null, null, "This relentless attack does massive damage to a target affected by status conditions."));
        Shingy.AddMove(new DingoMove(7, "Curse", "Dark", 0, 0, 100, null, null, "A move that works differently for the Ghost type than for all other types."));
        Shingy.AddMove(new DingoMove(8, "Shadow Punch", "Dark", 0, 60, 100, null, null, "The user throws a punch from the shadows. The punch lands without fail."));
        Shingy.AddMove(new DingoMove(9, "Dream Eater", "Dark", 0, 100, 100, null, null, "The user eats the dreams of a sleeping target. It absorbs half the damage caused to heal the user's HP."));

        SadCloud.AddMove(new DingoMove(0, "Misty Veil", "Wind", 0, 0, 100, null, null, "Surrounds itself with a veil of mist that reduces incoming damage."));
        SadCloud.AddMove(new DingoMove(1, "Cloud Burst", "Wind", 0, 100, 90, null, null, "Gathers moisture from the air to create a powerful burst of cloud energy."));
        SadCloud.AddMove(new DingoMove(2, "Drizzle", "Water", 0, 0, 100, null, null, "Summons a light rain that boosts the power of water-type moves."));
        SadCloud.AddMove(new DingoMove(3, "Misty Terrain", "Wind", 0, 0, 100, null, null, "The user covers the ground under everyone's feet with mist for five turns. This protects Pokémon on the ground from status conditions."));
        SadCloud.AddMove(new DingoMove(4, "Tailwind", "Wind", 0, 0, 100, null, null, "The user whips up a turbulent whirlwind that boosts its Speed stat and the Speed stats of its allies for four turns."));
        SadCloud.AddMove(new DingoMove(5, "Hurricane", "Wind", 0, 110, 70, null, null, "The user attacks by wrapping its opponent in a fierce wind that flies up into the sky. This may also confuse the target."));
        SadCloud.AddMove(new DingoMove(6, "Air Slash", "Wind", 0, 75, 95, null, null, "The user attacks with a blade of air that slices even the sky."));
        SadCloud.AddMove(new DingoMove(7, "Gust", "Wind", 0, 40, 100, null, null, "A burst of high-speed air strikes the target with speed."));
        SadCloud.AddMove(new DingoMove(8, "Whirlwind", "Wind", 0, 0, 100, null, null, "The target is blown away, to be replaced by another Pokémon in its party."));
        SadCloud.AddMove(new DingoMove(9, "Defog", "Wind", 0, 0, 100, null, null, "Removes entry hazards, light screens, and reflects on both sides of the field."));

        Worm.AddMove(new DingoMove(0, "Tunnel Strike", "Physical", 0, 70, 100, null, null, "Emerges from underground to deliver a surprise strike to the opponent."));
        Worm.AddMove(new DingoMove(1, "Sand Tomb", "Physical", 0, 35, 85, null, null, "Traps the target inside a harshly raging sandstorm for four to five turns."));
        Worm.AddMove(new DingoMove(2, "Earthquake", "Physical", 0, 100, 100, null, null, "Causes a powerful earthquake that damages all Pokémon on the field."));
        Worm.AddMove(new DingoMove(3, "Mud-Slap", "Physical", 0, 20, 100, null, null, "The user hurls mud in the target's face to inflict damage and lower its accuracy."));
        Worm.AddMove(new DingoMove(4, "Dig", "Physical", 0, 80, 100, null, null, "The user burrows underground, then attacks on the next turn."));
        Worm.AddMove(new DingoMove(5, "Bulldoze", "Physical", 0, 60, 100, null, null, "The user strikes everything around it by stomping down on the ground. This lowers the Speed stat of those hit."));
        Worm.AddMove(new DingoMove(6, "Sand Attack", "Physical", 0, 0, 100, null, null, "Sand is hurled in the target's face, reducing its accuracy."));
        Worm.AddMove(new DingoMove(7, "Spikes", "Physical", 0, 0, 100, null, null, "The user lays a trap of spikes at the opposing team's feet. The trap hurts Pokémon that switch into battle."));
        Worm.AddMove(new DingoMove(8, "Mud Bomb", "Physical", 0, 65, 85, null, null, "The user launches a hard-packed mud ball to attack. This may also lower the target's accuracy."));
        Worm.AddMove(new DingoMove(9, "Sandstorm", "Physical", 0, 0, 100, null, null, "Whips up a sandstorm that rages for four to five turns."));

        Pebble.AddMove(new DingoMove(0, "Stone Barrage", "Physical", 0, 85, 90, null, null, "Launches a barrage of stones at the opponent with incredible speed."));
        Pebble.AddMove(new DingoMove(1, "Dark Gravel", "Dark", 0, 80, 100, null, null, "Hurls sharp, darkened gravel at the opponent to inflict damage and lower accuracy."));
        Pebble.AddMove(new DingoMove(2, "Shatter Shield", "Physical", 0, 0, 100, null, null, "Forms a shield of hardened rock that reduces incoming damage and reflects certain moves."));
        Pebble.AddMove(new DingoMove(3, "Shadow Strike", "Dark", 0, 70, 100, null, null, "The user strikes the target with a shadowy force, sometimes causing the target to flinch."));
        Pebble.AddMove(new DingoMove(4, "Demon Wing Slash", "Dark", 0, 85, 90, null, null, "The user slashes the target with its demonic wings, leaving behind dark energy that may cause additional damage over time."));
        Pebble.AddMove(new DingoMove(5, "Nightfall", "Dark", 0, 0, 100, null, null, "The user shrouds the battlefield in darkness, reducing the accuracy of all non-Dark moves."));
        Pebble.AddMove(new DingoMove(6, "Rock Polish", "Physical", 0, 0, 100, null, null, "The user polishes its body to reduce drag, raising its Speed stat."));
        Pebble.AddMove(new DingoMove(7, "Stone Edge", "Physical", 0, 100, 80, null, null, "Hurls sharp stones at the opponent with incredible precision."));
        Pebble.AddMove(new DingoMove(8, "Tectonic Slam", "Physical", 0, 120, 70, null, null, "Slams the ground with tremendous force, causing a massive shockwave."));
        Pebble.AddMove(new DingoMove(9, "Crystal Barrier", "Physical", 0, 0, 100, null, null, "Creates a barrier of crystal that reflects incoming attacks."));

        Ghost.AddMove(new DingoMove(0, "Ethereal Slash", "Dark", 0, 70, 100, null, null, "The user slashes the target with ethereal energy. Critical hits land more easily."));
        Ghost.AddMove(new DingoMove(1, "Spectral Illusion", "Dark", 0, 0, 100, null, null, "The user creates a frightening illusion to confuse the target. It inflicts damage matching the user's level."));
        Ghost.AddMove(new DingoMove(2, "Haunting Hex", "Dark", 0, 65, 100, null, null, "The user curses the target with a haunting hex. If the target is affected by a status condition, this move has double power."));
        Ghost.AddMove(new DingoMove(3, "Wraithfire", "Dark", 0, 0, 85, null, null, "The user unleashes a sinister, spectral flame at the target to inflict a burn."));
        Ghost.AddMove(new DingoMove(4, "Phantom Bolt", "Dark", 0, 80, 100, null, null, "The user hurls a ghostly bolt of energy at the target. This may also lower the target's Defense stat."));
        Ghost.AddMove(new DingoMove(5, "Dark Curse", "Dark", 0, 0, 100, null, null, "A move that works differently for the Dark type than for all other types."));
        Ghost.AddMove(new DingoMove(6, "Nightmare Realm", "Dark", 0, 0, 100, null, null, "A sleeping target is tormented by visions from the nightmare realm, inflicting damage every turn."));
        Ghost.AddMove(new DingoMove(7, "Shadow Step", "Dark", 0, 40, 100, null, null, "The user vanishes into the shadows and attacks the target from behind. This move always goes first."));
        Ghost.AddMove(new DingoMove(8, "Soul Strike", "Dark", 0, 60, 100, null, null, "The user delivers a powerful strike infused with spiritual energy. This attack never misses."));
        Ghost.AddMove(new DingoMove(9, "Soul Drain", "Dark", 0, 0, 100, null, null, "The user drains the target's spiritual energy, cutting 4 PP from the last move used by the target."));

        Bulb.AddMove(new DingoMove(0, "Dark Surge", "Dark", 0, 80, 100, null, null, "Unleashes a surge of dark energy to damage the opponent."));
        Bulb.AddMove(new DingoMove(1, "Shockwave", "Lightning", 0, 70, 100, null, null, "Emits a powerful shockwave of electricity to paralyze the opponent."));
        Bulb.AddMove(new DingoMove(2, "Shadow Bolt", "Dark", 0, 90, 95, null, null, "Fires a bolt of dark electricity that can cause confusion."));
        Bulb.AddMove(new DingoMove(3, "Eerie Glow", "Dark", 0, 0, 100, null, null, "Emits an eerie glow that lowers the opponent's accuracy."));
        Bulb.AddMove(new DingoMove(4, "Volt Absorb", "Lightning", 0, 0, 100, null, null, "Absorbs electricity to restore health."));
        Bulb.AddMove(new DingoMove(5, "Nightshade", "Dark", 0, 60, 100, null, null, "Summons shadows to attack the opponent."));
        Bulb.AddMove(new DingoMove(6, "Electric Surge", "Lightning", 0, 90, 95, null, null, "Creates a surge of electricity to damage the opponent."));
        Bulb.AddMove(new DingoMove(7, "Shadow Strike", "Dark", 0, 100, 80, null, null, "Launches a powerful strike from the shadows."));
        Bulb.AddMove(new DingoMove(8, "Dark Pulse", "Dark", 0, 80, 100, null, null, "Unleashes a pulse of dark energy to damage the opponent."));
        Bulb.AddMove(new DingoMove(9, "Volt Switch", "Lightning", 0, 70, 100, null, null, "Attacks the opponent with electricity before switching out."));

        // Moves for Crick
        Crick.AddMove(new DingoMove(0, "Bug Bite", "Grass", 0, 60, 100, null, null, "The user bites the target. If the target is holding a Berry, the user eats it and gains its effect."));
        Crick.AddMove(new DingoMove(1, "String Shot", "Grass", 0, 0, 95, null, null, "The opposing Pokémon are bound with silk blown from the user's mouth that harshly lowers the Speed stat."));
        Crick.AddMove(new DingoMove(2, "Leech Life", "Grass", 0, 80, 100, null, null, "The user drains the target's blood. The user's HP is restored by half the damage taken by the target."));
        Crick.AddMove(new DingoMove(3, "Poison Sting", "Grass", 0, 15, 100, null, null, "The user stabs the target with a poisonous stinger. This may also poison the target."));
        Crick.AddMove(new DingoMove(4, "Bug Buzz", "Grass", 0, 90, 100, null, null, "The user generates a damaging sound wave by vibration. This may also lower the target's Special Defense stat."));
        Crick.AddMove(new DingoMove(5, "Sticky Web", "Grass", 0, 0, 100, null, null, "The user weaves a sticky net around the opposing team, which lowers their Speed stats upon switching into battle."));
        Crick.AddMove(new DingoMove(6, "Pin Missile", "Grass", 0, 25, 95, null, null, "Sharp spikes are shot at the target in rapid succession. They hit two to five times in a row."));
        Crick.AddMove(new DingoMove(7, "X-Scissor", "Grass", 0, 80, 100, null, null, "The user slashes at the target by crossing its scythes or claws as if they were a pair of scissors."));
        Crick.AddMove(new DingoMove(8, "Infestation", "Grass", 0, 20, 100, null, null, "The target becomes trapped within a maelstrom of energy, damaging it every turn for four to five turns."));
        Crick.AddMove(new DingoMove(9, "Quiver Dance", "Grass", 0, 0, 100, null, null, "The user lightly performs a beautiful, mystic dance. This boosts the user's Special Attack, Special Defense, and Speed stats."));

        // Moves for Firefly
        Firefly.AddMove(new DingoMove(0, "Ember", "Fire", 0, 40, 100, null, null, "The target is attacked with small flames. This may also leave the target with a burn."));
        Firefly.AddMove(new DingoMove(1, "Fire Spin", "Fire", 0, 35, 85, null, null, "The target becomes trapped within a fierce vortex of fire that rages for four to five turns."));
        Firefly.AddMove(new DingoMove(2, "Flame Charge", "Fire", 0, 50, 100, null, null, "Cloaked in flames, the user charges at the target. This may also leave the target with a burn."));
        Firefly.AddMove(new DingoMove(3, "Inferno", "Fire", 0, 100, 50, null, null, "The user attacks by engulfing the target in an intense fire. This leaves the target with a burn."));
        Firefly.AddMove(new DingoMove(4, "Flamethrower", "Fire", 0, 90, 100, null, null, "The target is scorched with an intense blast of fire. This may also leave the target with a burn."));
        Firefly.AddMove(new DingoMove(5, "Heat Wave", "Fire", 0, 95, 90, null, null, "The user attacks by exhaling hot breath on the opposing team. This may also leave the targets with a burn."));
        Firefly.AddMove(new DingoMove(6, "Fire Blast", "Fire", 0, 110, 85, null, null, "The target is attacked with an intense blast of all-consuming fire. This may also leave the target with a burn."));
        Firefly.AddMove(new DingoMove(7, "Flare Blitz", "Fire", 0, 120, 100, null, null, "The user cloaks itself in fire and charges at the target. This may also leave the target with a burn."));
        Firefly.AddMove(new DingoMove(8, "Overheat", "Fire", 0, 130, 90, null, null, "The user attacks the target at full power. The attack's recoil harshly lowers the user's Sp. Atk stat."));
        Firefly.AddMove(new DingoMove(9, "Burn Up", "Fire", 0, 130, 100, null, null, "To inflict massive damage, the user burns itself out. After using this move, the user will no longer be Fire type."));

        // Moves for Freddy
        Freddy.AddMove(new DingoMove(0, "Nightmare", "Abnormal", 0, 0, 100, null, null, "A sleeping target sees a nightmare that inflicts some damage every turn."));
        Freddy.AddMove(new DingoMove(1, "Shadow Sneak", "Abnormal", 0, 40, 100, null, null, "The user extends its shadow and attacks the target from behind. This move always goes first."));
        Freddy.AddMove(new DingoMove(2, "Dream Eater", "Abnormal", 0, 100, 100, null, null, "The user eats the dreams of a sleeping target. It absorbs half the damage caused to heal the user's HP."));
        Freddy.AddMove(new DingoMove(3, "Confuse Ray", "Abnormal", 0, 0, 100, null, null, "The target is exposed to a sinister ray that triggers confusion."));
        Freddy.AddMove(new DingoMove(4, "Hex", "Abnormal", 0, 65, 100, null, null, "This relentless attack does massive damage to a target affected by status conditions."));
        Freddy.AddMove(new DingoMove(5, "Curse", "Abnormal", 0, 0, 100, null, null, "A move that works differently for the Ghost type than for all other types."));
        Freddy.AddMove(new DingoMove(6, "Hypnosis", "Abnormal", 0, 0, 60, null, null, "The user employs hypnotic suggestion to make the target fall into a deep sleep."));
        Freddy.AddMove(new DingoMove(7, "Pain Split", "Abnormal", 0, 0, 100, null, null, "The user adds its HP to the target's HP, then equally shares the combined HP with the target."));
        Freddy.AddMove(new DingoMove(8, "Night Shade", "Abnormal", 0, 0, 100, null, null, "The user makes the target see a frightening mirage. It inflicts damage matching the user's level."));
        Freddy.AddMove(new DingoMove(9, "Spite", "Abnormal", 0, 0, 100, null, null, "The user unleashes its grudge on the move last used by the target by cutting 4 PP from it."));

        // Moves for Fried
        Fried.AddMove(new DingoMove(0, "Psycho Cut", "Abnormal", 0, 70, 100, null, null, "The user tears at the target with blades formed by psychic power. Critical hits land more easily."));
        Fried.AddMove(new DingoMove(1, "Psyshock", "Abnormal", 0, 80, 100, null, null, "The user materializes an odd psychic wave to attack the target. This attack does physical damage."));
        Fried.AddMove(new DingoMove(2, "Confusion", "Abnormal", 0, 50, 100, null, null, "The target is hit by a weak telekinetic force. This may also confuse the target."));
        Fried.AddMove(new DingoMove(3, "Future Sight", "Abnormal", 0, 120, 100, null, null, "Two turns after this move is used, a hunk of psychic energy attacks the target."));
        Fried.AddMove(new DingoMove(4, "Zen Headbutt", "Abnormal", 0, 80, 90, null, null, "The user focuses its willpower to its head and attacks the target. This may also make the target flinch."));
        Fried.AddMove(new DingoMove(5, "Miracle Eye", "Abnormal", 0, 0, 100, null, null, "Enables a Dark-type target to be hit by Psychic-type attacks. It also enables an evasive target to be hit."));
        Fried.AddMove(new DingoMove(6, "Psybeam", "Abnormal", 0, 65, 100, null, null, "The target is attacked with a peculiar ray. This may also leave the target confused."));
        Fried.AddMove(new DingoMove(7, "Calm Mind", "Abnormal", 0, 0, 100, null, null, "The user quietly focuses its mind and calms its spirit to raise its Sp. Atk and Sp. Def stats."));
        Fried.AddMove(new DingoMove(8, "Extrasensory", "Abnormal", 0, 80, 100, null, null, "The user attacks with an odd, unseeable power. This may also make the target flinch."));
        Fried.AddMove(new DingoMove(9, "Trick Room", "Abnormal", 0, 0, 100, null, null, "The user creates a bizarre area in which slower Pokémon get to move first for five turns."));

        // Moves for Icemunchkin
        Icemunchkin.AddMove(new DingoMove(0, "Ice Shard", "Ice", 0, 40, 100, null, null, "The user flash-freezes chunks of ice and hurls them at the target. This move always goes first."));
        Icemunchkin.AddMove(new DingoMove(1, "Aurora Beam", "Ice", 0, 65, 100, null, null, "The target is hit with a rainbow-colored beam. This may also lower the target's Attack stat."));
        Icemunchkin.AddMove(new DingoMove(2, "Ice Punch", "Ice", 0, 75, 100, null, null, "The target is punched with an icy fist. This may also leave the target frozen."));
        Icemunchkin.AddMove(new DingoMove(3, "Ice Beam", "Ice", 0, 90, 100, null, null, "The target is struck with an icy-cold beam of energy. This may also freeze the target."));
        Icemunchkin.AddMove(new DingoMove(4, "Blizzard", "Ice", 0, 110, 70, null, null, "A howling blizzard is summoned to strike opposing Pokémon. This may also freeze them."));
        Icemunchkin.AddMove(new DingoMove(5, "Frost Breath", "Ice", 0, 60, 90, null, null, "The user blows its cold breath on the target. This attack always results in a critical hit."));
        Icemunchkin.AddMove(new DingoMove(6, "Ice Fang", "Ice", 0, 65, 95, null, null, "The user bites with cold-infused fangs. This may also make the target flinch or leave it frozen."));
        Icemunchkin.AddMove(new DingoMove(7, "Icicle Spear", "Ice", 0, 25, 100, null, null, "The user launches sharp icicles at the target. It strikes two to five times in a row."));
        Icemunchkin.AddMove(new DingoMove(8, "Freeze-Dry", "Ice", 0, 70, 100, null, null, "The user rapidly cools the target. This may also leave the target frozen."));
        Icemunchkin.AddMove(new DingoMove(9, "Ice Shard", "Ice", 0, 40, 100, null, null, "The user flash-freezes chunks of ice and hurls them at the target. This move always goes first."));

        // Moves for Octi
        Octi.AddMove(new DingoMove(0, "Water Gun", "Water", 0, 40, 100, null, null, "The target is blasted with a forceful shot of water."));
        Octi.AddMove(new DingoMove(1, "Bubble", "Water", 0, 40, 100, null, null, "A spray of countless bubbles is jetted at the opposing Pokémon. This may also lower their Speed stats."));
        Octi.AddMove(new DingoMove(2, "Octazooka", "Water", 0, 65, 85, null, null, "The user attacks by spraying ink in the target's face or eyes. This may also lower the target's accuracy."));
        Octi.AddMove(new DingoMove(3, "Aqua Jet", "Water", 0, 40, 100, null, null, "The user lunges at the target at a speed that makes it almost invisible. This move always goes first."));
        Octi.AddMove(new DingoMove(4, "Water Pulse", "Water", 0, 60, 100, null, null, "The user attacks the target with a pulsing blast of water. This may also confuse the target."));
        Octi.AddMove(new DingoMove(5, "Bubble Beam", "Water", 0, 65, 100, null, null, "A spray of bubbles is forcefully ejected at the target. This may also lower its Speed stat."));
        Octi.AddMove(new DingoMove(6, "Hydro Pump", "Water", 0, 110, 80, null, null, "The target is blasted by a huge volume of water launched under great pressure."));
        Octi.AddMove(new DingoMove(7, "Surf", "Water", 0, 90, 100, null, null, "The user attacks everything around it by swamping its surroundings with a giant wave."));
        Octi.AddMove(new DingoMove(8, "Waterfall", "Water", 0, 80, 100, null, null, "The user charges at the target and may make it flinch."));
        Octi.AddMove(new DingoMove(9, "Liquidation", "Water", 0, 85, 100, null, null, "The user slams into the target using a full-force blast of water. This may also lower the target's Defense stat."));

        // Moves for Tanktop
        Tanktop.AddMove(new DingoMove(0, "Mind Crush", "Abnormal", 0, 70, 100, null, null, "Invades the target's mind with overwhelming psychic force, potentially causing confusion."));
        Tanktop.AddMove(new DingoMove(1, "Psycho Slam", "Abnormal", 0, 85, 95, null, null, "Slams the target with psychic energy, potentially causing confusion and disorientation."));
        Tanktop.AddMove(new DingoMove(2, "Nightmare Punch", "Abnormal", 0, 90, 100, null, null, "Delivers a punch infused with dark energy, causing the opponent to experience nightmares if it hits."));
        Tanktop.AddMove(new DingoMove(3, "Chaos Strike", "Abnormal", 0, 100, 90, null, null, "Unleashes a chaotic attack that has a chance to confuse or paralyze the target."));
        Tanktop.AddMove(new DingoMove(4, "Hex Crush", "Abnormal", 0, 80, 100, null, null, "Crushes the target with hexed energy, potentially causing various abnormal status conditions."));
        Tanktop.AddMove(new DingoMove(5, "Mind Warp", "Abnormal", 0, 0, 100, null, null, "Warps the target's perception of reality, potentially causing confusion or disorientation."));
        Tanktop.AddMove(new DingoMove(6, "Psychic Surge", "Abnormal", 0, 95, 90, null, null, "Creates a surge of psychic energy that can disrupt the target's mental state and lower its defenses."));
        Tanktop.AddMove(new DingoMove(7, "Ethereal Crush", "Abnormal", 0, 110, 100, null, null, "Crushes the target with ethereal energy, potentially causing it to phase out of reality momentarily."));
        Tanktop.AddMove(new DingoMove(8, "Nightmare Crush", "Abnormal", 0, 120, 80, null, null, "Crushes the target with a nightmarish force, potentially causing it to experience terrifying visions."));
        Tanktop.AddMove(new DingoMove(9, "Disorienting Strike", "Abnormal", 0, 75, 100, null, null, "Strikes the target in a disorienting manner, potentially confusing or stunning it."));

        // Moves for TrustFundBaby
        TrustFundBaby.AddMove(new DingoMove(0, "Lay Offs", "Finance", 0, 0, 100, null, null, "Permanently removes a fainted or low-level Bingomon from your party for a large one-time cash payout."));
        TrustFundBaby.AddMove(new DingoMove(1, "Market Analysis", "Finance", 0, 0, 100, "marketanalysis", null, "Double cash gained for the rest of the battle."));
        TrustFundBaby.AddMove(new DingoMove(2, "ATM Withdrawal", "Finance", 0, 0, 100, null, null, "Pull some money out of your savings."));
        TrustFundBaby.AddMove(new DingoMove(3, "Bear Market", "Finance", 0, 0, 100, null, null, "Creates an unfavorable market condition, lowering the opponent's Attack and Special Attack stats."));
        TrustFundBaby.AddMove(new DingoMove(4, "Stock Buyback", "Finance", 0, 0, 100, null, null, "Repurchases its own HP, restoring a portion of its health."));
        TrustFundBaby.AddMove(new DingoMove(5, "Hostile Takeover", "Finance", 0, 0, 100, null, null, "Seizes control of the battle, forcing the opponent to switch out to another Pokémon."));
        TrustFundBaby.AddMove(new DingoMove(6, "Insider Trading", "Finance", 0, 0, 100, null, null, "Gains insight into the opponent's next move, allowing it to act first in the next turn."));
        TrustFundBaby.AddMove(new DingoMove(7, "Dividend Payout", "Finance", 0, 0, 100, null, null, "Rewards itself with a portion of the opponent's HP, draining their health."));
        TrustFundBaby.AddMove(new DingoMove(8, "Tax Haven", "Finance", 0, 0, 100, null, null, "Creates a financial shelter, reducing the damage it takes from the opponent's next attack."));
        TrustFundBaby.AddMove(new DingoMove(9, "Golden Parachute", "Finance", 0, 0, 100, null, null, "Secures a safety net, guaranteeing it won't faint from the next damaging move it receives."));

        Plant.AddMove(new DingoMove(0, "Photosynthesis Beam", "Grass", 0, 90, 100, null, null, "Emits a beam of pure sunlight, nourishing itself and damaging the opponent."));
        Plant.AddMove(new DingoMove(1, "Vine Whip Wrap", "Grass", 0, 80, 95, null, null, "Wraps the opponent tightly with supple vines, restraining them and dealing damage over time."));
        Plant.AddMove(new DingoMove(2, "Rooted Resilience", "Grass", 0, 0, 100, null, null, "Imbues itself with the strength of the earth, increasing its Defense and Special Defense stats for a few turns."));
        Plant.AddMove(new DingoMove(3, "Pollen Burst", "Grass", 0, 70, 100, null, null, "Bursts open with a cloud of pollen, causing irritation and damaging the opponent."));
        Plant.AddMove(new DingoMove(4, "Blossom Shield", "Grass", 0, 0, 100, null, null, "Surrounds itself with a shield of vibrant blossoms, reducing incoming damage from physical attacks."));
        Plant.AddMove(new DingoMove(5, "Fungal Spores", "Grass", 0, 75, 90, null, null, "Scatters spores that infect the opponent, causing gradual damage and reducing their Speed stat."));
        Plant.AddMove(new DingoMove(6, "Thorned Trap", "Grass", 0, 85, 100, null, null, "Sets a hidden trap of sharp thorns, damaging the opponent when they attempt to attack."));
        Plant.AddMove(new DingoMove(7, "Canopy Cover", "Grass", 0, 0, 100, null, null, "Creates a protective canopy of leaves, reducing incoming damage from special attacks."));
        Plant.AddMove(new DingoMove(8, "Dewdrop Defense", "Grass", 0, 0, 100, null, null, "Covers itself in a refreshing dew, raising its evasion and reducing the accuracy of the opponent's next move."));
        Plant.AddMove(new DingoMove(9, "Seedling Surge", "Grass", 0, 95, 100, null, null, "Channels the energy of nature into a powerful surge of growth, dealing massive damage to the opponent."));

        Waterslime.AddMove(new DingoMove(0, "Water Cannon", "Water", 0, 30, 100, null, null, "The target is blasted with a forceful shot of water."));
        Waterslime.AddMove(new DingoMove(1, "Hydro Slice", "Water", 0, 40, 90, null, null, ""));
        Waterslime.AddMove(new DingoMove(2, "Aqua Shield", "Water", 0, 0, 100, null, null, "Forms a protective shield of water to reduce incoming damage."));
        Waterslime.AddMove(new DingoMove(3, "Look Sad", "Water", 0, 0, 100, null, null, "The user gives a pitiful look, causing the opponent's morale to plummet, lowering the oponent's defense."));
        Waterslime.AddMove(new DingoMove(4, "Nightfall Strike", "Dark", 0, 60, 100, null, null, "The user strikes the opponent with a dark energy infused attack, dealing damage."));
        Waterslime.AddMove(new DingoMove(5, "Hydro Beam", "Water", 0, 60, 100, null, null, "The user unleashes a concentrated beam of pressurized water at the opponent, dealing damage"));
        Waterslime.AddMove(new DingoMove(6, "Torrential Downpour", "Water", 0, 0, 100, null, null, "The user summons a torrential downpour that drenches the battlefield, boosting its attack."));
        Waterslime.AddMove(new DingoMove(7, "Tidal Wave", "Water", 0, 85, 100, null, null, "Creates a massive wave that crashes down on the opponent."));
        Waterslime.AddMove(new DingoMove(8, "Umbral Strike", "Dark", 0, 90, 95, null, null, "The user strikes the opponent with a dark, shadowy force, dealing damage"));
        Waterslime.AddMove(new DingoMove(9, "Hydro Vortex", "Water", 0, 100, 90, null, null, "The user creates a swirling vortex of water that engulfs the opponent, dealing damage"));

        Ducky.AddMove(new DingoMove(0, "Gale Blast", "Wind", 0, 50, 100, null, null, "The user unleashes a powerful blast of wind energy at the opponent."));
        Ducky.AddMove(new DingoMove(1, "Zephyr Slash", "Wind", 0, 70, 95, null, null, "The user slashes at the opponent with razor-sharp winds, cutting through defenses."));
        Ducky.AddMove(new DingoMove(2, "Whirlpool Whisk", "Wind", 0, 60, 100, null, null, "The user whisks up a whirlpool of wind around the opponent, dealing damage."));
        Ducky.AddMove(new DingoMove(3, "Breeze Burst", "Wind", 0, 80, 90, null, null, "The user releases a burst of refreshing breeze that restores its own health."));
        Ducky.AddMove(new DingoMove(4, "Tempest Toss", "Wind", 0, 90, 85, null, null, "The user tosses the opponent into a raging tempest, dealing damage."));
        Ducky.AddMove(new DingoMove(5, "Quack", "Abnormal", 0, 0, 100, null, null, "The user emits a bizarre quacking sound that confuses the opponent, potentially causing them to become disoriented."));
        Ducky.AddMove(new DingoMove(6, "Zephyr Strike", "Wind", 0, 75, 95, null, null, "The user charges forward with the speed of a zephyr, striking the opponent."));
        Ducky.AddMove(new DingoMove(7, "Cyclone Crush", "Wind", 0, 100, 80, null, null, "The user generates a powerful cyclone that crushes the opponent with immense force."));
        Ducky.AddMove(new DingoMove(8, "Whispering Wind", "Wind", 0, 40, 100, null, null, "The user releases a gentle whispering wind that lulls the opponent to sleep."));
        Ducky.AddMove(new DingoMove(9, "Breeze Blade", "Wind", 0, 65, 90, null, null, "The user slashes at the opponent with a blade of cutting wind, dealing damage."));

        Forqwa.AddMove(new DingoMove(0, "Lick", "Abnormal", 0, 30, 100, null, null, "The user licks the opponent with its rugged tongue, potentially paralyzing them with a tingling sensation."));
        Forqwa.AddMove(new DingoMove(8, "Earthquake Slam", "Physical", 0, 100, 100, null, null, "The user slams the ground with tremendous force, causing a powerful earthquake that damages all opposing Pokémon."));
        Forqwa.AddMove(new DingoMove(1, "Rock Smash", "Physical", 0, 40, 100, null, null, "The user smashes the target with a rock-hard fist, breaking through barriers like Reflect and Light Screen."));
        Forqwa.AddMove(new DingoMove(2, "Iron Fist", "Physical", 0, 70, 90, null, null, "The user's fist becomes as hard as iron, allowing it to deliver a powerful punch that may lower the target's Defense stat."));
        Forqwa.AddMove(new DingoMove(3, "Tectonic Charge", "Physical", 0, 80, 85, null, null, "The user charges forward with seismic energy, slamming into the target with great force."));
        Forqwa.AddMove(new DingoMove(4, "Stone Edge", "Physical", 0, 100, 80, null, null, "The user hurls sharp rocks at the target to strike two to five times in a row."));
        Forqwa.AddMove(new DingoMove(5, "Mudslide", "Physical", 0, 70, 95, null, null, "The user triggers a mudslide that engulfs the opponent, dealing damage and potentially lowering their accuracy."));
        Forqwa.AddMove(new DingoMove(6, "Meteor Smash", "Physical", 0, 110, 70, null, null, "The user smashes into the opponent like a meteor, dealing massive damage but lowering its own Speed."));
        Forqwa.AddMove(new DingoMove(7, "Boulder Toss", "Physical", 0, 85, 90, null, null, "The user tosses a massive boulder at the target, dealing damage and potentially causing flinching."));

        Flame.AddMove(new DingoMove(0, "Ember", "Fire", 0, 40, 100, null, null, "The target is attacked with small flames. This may also leave the target with a burn."));
        Flame.AddMove(new DingoMove(1, "Fire Spin", "Fire", 0, 35, 85, null, null, "The target becomes trapped within a fierce vortex of fire that rages for four to five turns."));
        Flame.AddMove(new DingoMove(2, "Flame Charge", "Fire", 0, 50, 100, null, null, "Cloaked in flames, the user charges at the target. This may also leave the target with a burn."));
        Flame.AddMove(new DingoMove(3, "Inferno", "Fire", 0, 100, 50, null, null, "The user attacks by engulfing the target in an intense fire. This leaves the target with a burn."));
        Flame.AddMove(new DingoMove(4, "Flamethrower", "Fire", 0, 90, 100, null, null, "The target is scorched with an intense blast of fire. This may also leave the target with a burn."));
        Flame.AddMove(new DingoMove(5, "Heat Wave", "Fire", 0, 95, 90, null, null, "The user attacks by exhaling hot breath on the opposing team. This may also leave the targets with a burn."));
        Flame.AddMove(new DingoMove(6, "Fire Blast", "Fire", 0, 110, 85, null, null, "The target is attacked with an intense blast of all-consuming fire. This may also leave the target with a burn."));
        Flame.AddMove(new DingoMove(7, "Flare Blitz", "Fire", 0, 120, 100, null, null, "The user cloaks itself in fire and charges at the target. This may also leave the target with a burn."));
        Flame.AddMove(new DingoMove(8, "Overheat", "Fire", 0, 130, 90, null, null, "The user attacks the target at full power. The attack's recoil harshly lowers the user's Sp. Atk stat."));
        Flame.AddMove(new DingoMove(9, "Burn Up", "Fire", 0, 130, 100, null, null, "To inflict massive damage, the user burns itself out. After using this move, the user will no longer be Fire type."));

        Doggofire.AddMove(new DingoMove(0, "Ember", "Fire", 0, 40, 100, null, null, "The target is attacked with small flames. This may also leave the target with a burn."));
        Doggofire.AddMove(new DingoMove(1, "Flame Charge", "Fire", 0, 50, 100, null, null, "The user cloaks itself in flames and charges. Boosts speed after use."));
        Doggofire.AddMove(new DingoMove(2, "Fire Fang", "Fire", 0, 65, 95, null, null, "The user bites with flame-cloaked fangs. May burn or flinch the target."));
        Doggofire.AddMove(new DingoMove(3, "Heat Wave", "Fire", 0, 95, 90, null, null, "The user exhales a scorching wave of heat. May burn targets."));
        Doggofire.AddMove(new DingoMove(4, "Inferno", "Fire", 0, 100, 50, null, null, "Engulfs the target in intense flames, guaranteeing a burn."));
        Doggofire.AddMove(new DingoMove(5, "Flare Blitz", "Fire", 0, 120, 100, null, null, "A reckless fire tackle. The user also takes recoil damage."));
        Doggofire.AddMove(new DingoMove(6, "Will-O-Wisp", "Fire", 0, 0, 85, null, null, "The user shoots a sinister flame to inflict a burn on the target."));
        Doggofire.AddMove(new DingoMove(7, "Fire Spin", "Fire", 0, 35, 85, null, null, "Traps the target in a fiery vortex for 4-5 turns."));
        Doggofire.AddMove(new DingoMove(8, "Overheat", "Fire", 0, 130, 90, null, null, "Powerful fire attack, but harshly lowers the user's Sp. Atk."));
        Doggofire.AddMove(new DingoMove(9, "Blast Burn", "Fire", 0, 150, 90, null, null, "A massive explosion. The user must rest next turn."));

        Doggoelectric.AddMove(new DingoMove(0, "Thunder Shock", "Lightning", 0, 40, 100, null, null, "A jolt of electricity that may paralyze the target."));
        Doggoelectric.AddMove(new DingoMove(1, "Spark", "Lightning", 0, 65, 100, null, null, "An electric charge that may paralyze the target."));
        Doggoelectric.AddMove(new DingoMove(2, "Thunder Fang", "Lightning", 0, 65, 95, null, null, "Bites with electrified fangs. May paralyze or flinch."));
        Doggoelectric.AddMove(new DingoMove(3, "Discharge", "Lightning", 0, 80, 100, null, null, "A shockwave that may paralyze all nearby foes."));
        Doggoelectric.AddMove(new DingoMove(4, "Thunderbolt", "Lightning", 0, 90, 100, null, null, "A strong electric blast. May paralyze the target."));
        Doggoelectric.AddMove(new DingoMove(5, "Volt Switch", "Lightning", 0, 70, 100, null, null, "Attacks, then switches out to another Dingo."));
        Doggoelectric.AddMove(new DingoMove(6, "Wild Charge", "Lightning", 0, 90, 100, null, null, "A reckless electric tackle. The user also takes recoil."));
        Doggoelectric.AddMove(new DingoMove(7, "Thunder Wave", "Lightning", 0, 0, 90, null, null, "Paralyzes the target, reducing its Speed."));
        Doggoelectric.AddMove(new DingoMove(8, "Zap Cannon", "Lightning", 0, 120, 50, null, null, "A powerful electric blast that always paralyzes."));
        Doggoelectric.AddMove(new DingoMove(9, "Thunder", "Lightning", 0, 110, 70, null, null, "A devastating thunderbolt. Hits even in rain."));

        Doggoice.AddMove(new DingoMove(0, "Powder Snow", "Ice", 0, 40, 100, null, null, "Blows a chilling snowstorm. May freeze the target."));
        Doggoice.AddMove(new DingoMove(1, "Ice Shard", "Ice", 0, 40, 100, null, null, "Flash-freezes ice chunks to hurl. Always goes first."));
        Doggoice.AddMove(new DingoMove(2, "Ice Fang", "Ice", 0, 65, 95, null, null, "Bites with icy fangs. May freeze or flinch the target."));
        Doggoice.AddMove(new DingoMove(3, "Aurora Beam", "Ice", 0, 65, 100, null, null, "A rainbow-colored beam. May lower Attack."));
        Doggoice.AddMove(new DingoMove(4, "Ice Beam", "Ice", 0, 90, 100, null, null, "A freezing blast. May freeze the target."));
        Doggoice.AddMove(new DingoMove(5, "Blizzard", "Ice", 0, 110, 70, null, null, "A howling blizzard. May freeze all foes."));
        Doggoice.AddMove(new DingoMove(6, "Hail", "Ice", 0, 0, 100, null, null, "Summons a hailstorm for 5 turns."));
        Doggoice.AddMove(new DingoMove(7, "Freeze-Dry", "Ice", 0, 70, 100, null, null, "Super-cold air. May freeze and is super-effective against Water."));
        Doggoice.AddMove(new DingoMove(8, "Icicle Crash", "Ice", 0, 85, 90, null, null, "Drops large icicles. May cause flinching."));
        Doggoice.AddMove(new DingoMove(9, "Sheer Cold", "Ice", 0, 0, 30, null, null, "A one-hit KO move if it hits."));

        Doggograss.AddMove(new DingoMove(0, "Vine Whip", "Grass", 0, 45, 100, null, null, "Strikes the target with slender vines."));
        Doggograss.AddMove(new DingoMove(1, "Razor Leaf", "Grass", 0, 55, 95, null, null, "Sharp-edged leaves slash the target. High crit ratio."));
        Doggograss.AddMove(new DingoMove(2, "Seed Bomb", "Grass", 0, 80, 100, null, null, "Slams a barrage of seeds down on the target."));
        Doggograss.AddMove(new DingoMove(3, "Synthesis", "Grass", 0, 0, 100, null, null, "Restores HP. Effectiveness varies with weather."));
        Doggograss.AddMove(new DingoMove(4, "Energy Ball", "Grass", 0, 90, 100, null, null, "A ball of Grass energy. May lower Sp. Def."));
        Doggograss.AddMove(new DingoMove(5, "Leaf Blade", "Grass", 0, 90, 100, null, null, "A sharp leaf slash. High crit ratio."));
        Doggograss.AddMove(new DingoMove(6, "Leech Seed", "Grass", 0, 0, 90, null, null, "Plants seeds that drain HP from the target."));
        Doggograss.AddMove(new DingoMove(7, "Wood Hammer", "Grass", 0, 120, 100, null, null, "A powerful tackle. The user also takes recoil."));
        Doggograss.AddMove(new DingoMove(8, "Solar Beam", "Grass", 0, 120, 100, null, null, "Charges sunlight, then fires a beam next turn."));
        Doggograss.AddMove(new DingoMove(9, "Petal Dance", "Grass", 0, 120, 100, null, null, "A frenzied dance. Confuses the user afterward."));

        Doggowater.AddMove(new DingoMove(0, "Water Gun", "Water", 0, 40, 100, null, null, "Blasts the target with a forceful shot of water."));
        Doggowater.AddMove(new DingoMove(1, "Bubble Beam", "Water", 0, 65, 100, null, null, "A spray of bubbles. May lower Speed."));
        Doggowater.AddMove(new DingoMove(2, "Aqua Jet", "Water", 0, 40, 100, null, null, "A speedy water tackle. Always goes first."));
        Doggowater.AddMove(new DingoMove(3, "Water Pulse", "Water", 0, 60, 100, null, null, "A pulsing blast. May confuse the target."));
        Doggowater.AddMove(new DingoMove(4, "Surf", "Water", 0, 90, 100, null, null, "A giant wave crashes down on all foes."));
        Doggowater.AddMove(new DingoMove(5, "Hydro Pump", "Water", 0, 110, 80, null, null, "A high-pressure water blast."));
        Doggowater.AddMove(new DingoMove(6, "Rain Dance", "Water", 0, 0, 100, null, null, "Summons rain for 5 turns, boosting Water moves."));
        Doggowater.AddMove(new DingoMove(7, "Aqua Tail", "Water", 0, 90, 90, null, null, "Swings a watery tail with great force."));
        Doggowater.AddMove(new DingoMove(8, "Scald", "Water", 0, 80, 100, null, null, "Boiling water. May burn the target."));
        Doggowater.AddMove(new DingoMove(9, "Water Spout", "Water", 0, 150, 100, null, null, "Power decreases if the user's HP is low."));

        BingoStar2.AddMove(new DingoMove(0, "Shooting Star", "Fire", 0, 35, 90, null, null, "Summons the power of the stars to strike the opponent."));
        BingoStar2.AddMove(new DingoMove(1, "Luminous Burst", "Light", 0, 40, 100, null, null, "A burst of radiant light that dazzles the opponent."));
        BingoStar2.AddMove(new DingoMove(2, "Eclipse", "Light", 0, 0, 100, null, null, "Conjures a temporary eclipse that shrouds the battlefield, increases light attack moves."));
        BingoStar2.AddMove(new DingoMove(3, "Cosmic Shield", "Light", 0, 0, 100, null, null, "Creates a protective shield using cosmic energy, raising defense sharply."));

        Marshmellow2.AddMove(new DingoMove(0, "Squishy Frenzy", "Abnormal", 0, 35, 100, null, null, "The user slams into the opponent with sweet force, causing damage."));
        Marshmellow2.AddMove(new DingoMove(1, "Squishy Frenzy", "Abnormal", 0, 0, 100, null, null, "The user forms a shield of sweetness, raising defense sharply."));
        Marshmellow2.AddMove(new DingoMove(2, "Squishy Frenzy", "Fire", 0, 20, 100, null, null, "The user throws a toasted marshmallow at the opponent, dealing damage."));
        Marshmellow2.AddMove(new DingoMove(3, "Squishy Frenzy", "Abnormal", 0, 0, 100, null, null, "The user gives the opponent a gooey glare, lowering their attack and defense."));
        Marshmellow2.AddMove(new DingoMove(4, "Squishy Frenzy", "Abnormal", 0, 35, 100, null, null, "The user slams into the opponent with sweet force, causing damage."));
        Marshmellow2.AddMove(new DingoMove(5, "Squishy Frenzy", "Abnormal", 0, 0, 100, null, null, "The user forms a shield of sweetness, raising defense sharply."));
        Marshmellow2.AddMove(new DingoMove(6, "Squishy Frenzy", "Fire", 0, 20, 100, null, null, "The user throws a toasted marshmallow at the opponent, dealing damage."));
        Marshmellow2.AddMove(new DingoMove(7, "Squishy Frenzy", "Abnormal", 0, 0, 100, null, null, "The user gives the opponent a gooey glare, lowering their attack and defense."));
        Marshmellow2.AddMove(new DingoMove(8, "Squishy Frenzy", "Abnormal", 0, 35, 100, null, null, "The user slams into the opponent with sweet force, causing damage."));
        Marshmellow2.AddMove(new DingoMove(9, "Squishy Frenzy", "Abnormal", 0, 0, 100, null, null, "The user forms a shield of sweetness, raising defense sharply."));
    }

    private static List<EnvironmentEffect> allEnvironemntEffects = new List<EnvironmentEffect>{
        Rain
    };
    public enum DingoListType
    {
        AllDingos,
        NewDingos,
        FinanceDingos,
        Marshmellow,
        SecretDingos,
        FireDingos,
        WaterDingos,
        TrainerDingos,
        marshmellow
    }
    public const int AllDingos = 0;
    public const int NewDingos = 1;
    public const int FinanceDingos = 2;
    public const int SecretDingos = 3;
    public const int FireDingos = 4;
    public const int WaterDingos = 5;
    public const int TrainerDingos = 6;
    public const int MarshmellowDingos = 99;

    public static List<DingoID> GetDingoList(int listType)
    {
        try
        {
            List<DingoID> result;

            switch (listType)
            {
                case DingoDatabase.AllDingos:
                    result = allDingos;
                    break;
                case DingoDatabase.NewDingos:
                    result = newDingos;
                    break;
                case DingoDatabase.FinanceDingos:
                    result = financeDingos;
                    break;
                case DingoDatabase.SecretDingos:
                    result = secretDingos;
                    break;
                case DingoDatabase.FireDingos:
                    result = fireDingos;
                    break;
                case DingoDatabase.WaterDingos:
                    result = waterDingos;
                    break;
                case DingoDatabase.TrainerDingos:
                    result = trainerDingos;
                    break;
                case DingoDatabase.MarshmellowDingos:
                    result = marshmellow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(listType), listType, "Invalid Dingo list type.");
            }

            // Debug each Dingo name
            Debug.Log($"[DingoDatabase] Retrieved list for type {listType} with {result.Count} Dingos:");
            foreach (var dingo in result)
            {
                Debug.Log($"- {dingo.Name}");
            }

            return result;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DingoDatabase] Error retrieving Dingo list: {ex.Message}");
            return null;
        }
    }
    public static List<DingoID> allDingos = new List<DingoID>
    {
        BingoStar,
        Bean,
        Bird,
        Pinkthing,
        Waterdino,
        Weirdtongue,
        Magicfishy,
        Marshmellow,
        Robbersnail,
        Rock,
        Seed,
        Buggy,
        DancingPlant,
        Shingy,
        SadCloud,
        Worm,
        Pebble,
        Ghost,
        Bulb,
        Crick,
        Firefly,
        Freddy,
        Fried,
        Icemunchkin,
        Octi,
        Tanktop,
        TrustFundBaby,
        Plant,
        Waterslime,
        Ducky,
        Forqwa,
        Flame,
        Doggoelectric,
        Doggofire,
        Doggoice,
        Doggograss,
        Doggowater

        // Add more Dingos as needed
    };
    public static List<DingoID> newDingos = new List<DingoID>
    {
        Icemunchkin,
        Marshmellow,
        Bird,
        BingoStar
        // Add more Dingos as needed
    };

    public static List<DingoID> financeDingos = new List<DingoID>
    {
        TrustFundBaby,
        Freddy
    };
    public static List<DingoID> marshmellow = new List<DingoID>
    {
        Marshmellow2
    };
    public static List<DingoID> agentBingo = new List<DingoID>
    {
        AgentBingo
    };
    public static List<DingoID> secretDingos = new List<DingoID>
    {
        Magicfishy,
        Tanktop
    };
    public static List<DingoID> fireDingos = new List<DingoID>
    {
        Marshmellow,
        Weirdtongue,
        Firefly,
        Flame
    };
    public static List<DingoID> waterDingos = new List<DingoID>
    {
        Waterdino,
        Waterslime,
        Ducky,
        Octi
    };
    public static List<DingoID> trainerDingos = new List<DingoID>
    {
        BingoStar2,
        Marshmellow2
    };
    public static int GetTotalDingos()
    {
        return allDingos.Count;
    }

    public static DingoID GetDingoAtIndex(int index)
    {
        if (index >= 0 && index < allDingos.Count)
        {
            return allDingos[index];
        }
        else
        {
            // Return a default Dingo or handle the out-of-range index as needed
            return null;
        }
    }
    public static DingoID GetDingoByID(int id)
    {
        // Iterate through each property in the DingoDatabase class
        foreach (var property in typeof(DingoDatabase).GetProperties())
        {
            // Check if the property is of type DingoID
            if (property.PropertyType == typeof(DingoID))
            {
                // Get the value of the property (which is a DingoID object)
                DingoID dingo = (DingoID)property.GetValue(null);

                // Check if the ID of the Dingo matches the provided ID
                if (dingo.ID == id)
                {
                    // Return the DingoID object
                    return dingo;
                }
            }
        }

        // If no matching Dingo is found, return null
        return null;
    }
    public static string GetDingoDescriptionByID(int id)
    {
        // Iterate through each property in the DingoDatabase class
        foreach (var property in typeof(DingoDatabase).GetProperties())
        {
            // Check if the property is of type DingoID
            if (property.PropertyType == typeof(DingoID))
            {
                // Get the value of the property (which is a DingoID object)
                DingoID dingo = (DingoID)property.GetValue(null);

                // Check if the ID of the Dingo matches the provided ID
                if (dingo.ID == id)
                {
                    // Return the DingoID object
                    return dingo.Description;
                }
            }
        }

        // If no matching Dingo is found, return null
        return null;
    }
    public static DingoMove GetMoveByID(int id, DingoID dingo)
    {
        Debug.Log($"Getting move ID {id} for {dingo.Name}");
        // Iterate through each move in the BingoStar object
        foreach (DingoMove move in dingo.Moves)
        {
            // Check if the ID of the move matches the provided ID
            if (move.MoveID == id)
            {
                Debug.Log($"Getting move ID {id} was {move.Name}");
                // Return the move
                return move;
            }
        }

        // If no matching move is found, return null
        return null;
    }


}

