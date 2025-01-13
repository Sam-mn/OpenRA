#region Copyright & License Information
/*
 * Copyright (c) The OpenRA Developers and Contributors
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Graphics;
using OpenRA.Mods.Common.Lint;
using OpenRA.Traits;
using OpenRA.Widgets;

namespace OpenRA.Mods.Common.Widgets.Logic.Ingame
{
	[ChromeLogicArgsHotkeys("CycleUnitsKey")]
	public class CycleUnitsHotkeyLogic : SingleHotkeyBaseLogic
	{
		readonly World world;
		readonly WorldRenderer worldRenderer;
		readonly ISelection selection;
		readonly Viewport viewport;

		[ObjectCreator.UseCtor]
		public CycleUnitsHotkeyLogic(Widget widget, ModData modData, WorldRenderer worldRenderer, World world, Dictionary<string, MiniYaml> logicArgs)
			: base(widget, modData, "CycleUnitsKey", "WORLD_KEYHANDLER", logicArgs)
		{
			this.world = world;
			viewport = worldRenderer.Viewport;
			this.worldRenderer = worldRenderer;
			selection = world.Selection;
		}

		protected override bool OnHotkeyActivated(KeyInput e)
		{
			if (world.IsGameOver)
				return false;

			var eligiblePlayers = SelectionUtils.GetPlayersToIncludeInSelection(world);

			var units = SelectionUtils.SelectActorsOnScreen(world, worldRenderer, null, eligiblePlayers).SubsetWithHighestSelectionPriority(e.Modifiers).ToList();

			if (units.Count == 0)
				return true;

			var next = units
				.SkipWhile(b => !selection.Contains(b))
				.Skip(1)
				.FirstOrDefault();

			next ??= units[0];

			selection.Combine(world, new Actor[] { next }, false, true);
			viewport.Center(selection.Actors);

			return true;
		}
	}
}
