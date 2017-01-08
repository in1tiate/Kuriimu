﻿using KuriimuContract.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace KuriimuContract
{
	public static class Tools
	{
		public static string LoadFileFilters(Dictionary<string, IFileAdapter> fileAdapters)
		{
			List<string> extensions = new List<string>();
			List<string> types = new List<string>();

			// All Supported
			foreach (string key in fileAdapters.Keys)
				extensions.Add(fileAdapters[key].Extension.ToLower());
			types.Add("All Supported Files (" + string.Join(";", extensions.ToArray()) + ")|" + string.Join(";", extensions.ToArray()));

			// Individual
			foreach (string key in fileAdapters.Keys)
				types.Add(fileAdapters[key].Description + " (" + fileAdapters[key].Extension.ToLower() + ")|" + fileAdapters[key].Extension.ToLower());
			types.Add("All Files (*.*)|*.*");

			return string.Join("|", types.ToArray());
		}

		public static Dictionary<string, IGameHandler> LoadGameHandlers(ToolStripDropDownButton tsb, Image noGameIcon, EventHandler selectedIndexChanged)
		{
			tsb.DropDownItems.Clear();
			ToolStripMenuItem tsiNoGame = new ToolStripMenuItem("No Game", noGameIcon, selectedIndexChanged);
			tsb.DropDownItems.Add(tsiNoGame);
			tsb.Text = tsiNoGame.Text;
			tsb.Image = tsiNoGame.Image;

			Dictionary<string, IGameHandler> gameHandlers = new Dictionary<string, IGameHandler>();
			foreach (IGameHandler gameHandler in PluginLoader<IGameHandler>.LoadPlugins(Settings.Default.PluginDirectory, "game*.dll"))
			{
				gameHandlers.Add(gameHandler.Name, gameHandler);

				ToolStripMenuItem tsiGameHandler = new ToolStripMenuItem(gameHandler.Name, gameHandler.Icon, selectedIndexChanged);
				tsb.DropDownItems.Add(tsiGameHandler);
			}

			return gameHandlers;
		}

		public static void LoadSupportedEncodings(ComboBox cmb, Encoding encoding)
		{
			cmb.DisplayMember = "Text";
			cmb.ValueMember = "Value";
			cmb.DataSource = (from enc in Encoding.GetEncodings()
							  let name = enc.DisplayName
							  where name.Contains("ASCII")
								 || name.Contains("Shift-JIS")
								 || (name.Contains("Unicode") && !name.Contains("32"))
							  let newname = name.Replace("US-", "")
							  orderby newname
							  select new ListItem(newname, enc.GetEncoding()))
			.ToList();
			cmb.SelectedValue = encoding;
		}

		public static TreeNode FindNodeByIEntry(this TreeView tre, IEntry entry)
		{
			TreeNode result = null;

			foreach (TreeNode node in tre.Nodes)
			{
				if (node.Tag == entry)
					result = node;

				if (result == null)
					foreach (TreeNode subNode in node.Nodes)
						if (subNode.Tag == entry)
						{
							result = subNode;
							break;
						}

				if (result != null)
					break;
			}

			return result;
		}

		public static void SelectNodeByIEntry(this TreeView tre, IEntry entry)
		{
			TreeNode result = null;

			foreach (TreeNode node in tre.Nodes)
			{
				if (node.Tag == entry)
					result = node;

				if (result == null)
					foreach (TreeNode subNode in node.Nodes)
						if (subNode.Tag == entry)
						{
							result = subNode;
							break;
						}

				if (result != null)
					break;
			}

			tre.SelectedNode = result;
		}

		public static void DoubleBuffer(Control ctrl, bool doubleBuffered)
		{
			ctrl.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ctrl, doubleBuffered, null);
		}
	}
}