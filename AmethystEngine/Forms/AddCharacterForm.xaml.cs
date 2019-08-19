using AmethystEngine.Components;
using BixBite.Characters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmethystEngine.Forms
{
  /// <summary>
  /// Interaction logic for AddCharacterForm.xaml
  /// </summary>
  public partial class AddCharacterForm : Window
  {
		Character DesiredCharacter = new Character();
		ObservableCollection<EditorObject> Sprites = new ObservableCollection<EditorObject>();
		public delegate void HooKfunction(Character c);
		public HooKfunction AddToScene;


    public AddCharacterForm()
    {
      InitializeComponent();

			CharacterSprites_LB.ItemsSource = Sprites;

		}

		private void BImportFolder_CB_Click(object sender, RoutedEventArgs e)
		{
			if (bImportFolder_CB.IsChecked == true)
			{
				MainGrid.RowDefinitions[2].Height = new GridLength(30);
			}
			else if (bImportFolder_CB.IsChecked == false)
			{
				MainGrid.RowDefinitions[2].Height = new GridLength(0);
			}
		}

		private void AddSpriteToCharacter_BTN_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
			{
				FileName = "Sprite Image.Png", //default file 
				Title = "Import new sprite image",
				DefaultExt = "All files (*.*)|*.*", //default file extension
				Filter = "png file (*.png)|*.png|All files (*.*)|*.*"
			};

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results kicks out if the user doesn't select an item.
			String importfilename, destfilename = "";
			if (result == true)
			{
				importfilename = dlg.FileName;
			}
			else return;
			String s = dlg.FileName.Substring(dlg.FileName.LastIndexOfAny(new char[] { '/', '\\' })+1);
			s.Substring(0, s.IndexOf('.')-1);
			Sprites.Add(new EditorObject(dlg.FileName, s,false,EObjectType.File));
		}

		private void GetCharacterFolderLoc_BTN_Click(object sender, RoutedEventArgs e)
		{
			Sprites.Clear();
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				Title = "Folder Location for this characters Sprites",
				FileName = "IGNORE THIS" //default file name
			};
			Nullable<bool> result = dlg.ShowDialog();
			// Process save file dialog box results
			string filename = "";
			if (result == true)
			{
				// Save document
				filename = dlg.FileName;
				filename = filename.Substring(0, filename.LastIndexOfAny(new Char[] { '/', '\\' }));
			}
			else return;
			Console.WriteLine(filename);

			CharacterFolderLocation_TB.Text = filename;

			foreach (String filepath in getFilesInDir(filename))
			{
				String s = filepath.Substring(filename.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
				s.Substring(0, s.IndexOf('.') - 1);
				Sprites.Add(new EditorObject(filepath, s, false, EObjectType.File));
				
			}

		}

		private void CharacterName_TB_KeyDown(object sender, KeyEventArgs e)
		{
			if(Key.Enter == e.Key)
			{
				DesiredCharacter.Name = CharacterName_TB.Text;
			}
		}


		static List<string> getFilesInDir(string dirPath)
		{
			List<string> retVal = new List<string>();
			try
			{
				retVal = System.IO.Directory.GetFiles(dirPath, "*.*", System.IO.SearchOption.TopDirectoryOnly).ToList();
				foreach (System.IO.DirectoryInfo d in new System.IO.DirectoryInfo(dirPath).GetDirectories("*", System.IO.SearchOption.TopDirectoryOnly))
				{
					retVal.AddRange(getFilesInDir(d.FullName));
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine(dirPath);
			}
			return retVal;
		}

		private void LBind_DragMove(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}

		private void Confirm_BTN_Click(object sender, RoutedEventArgs e)
		{
			foreach(EditorObject eobj in Sprites)
				DesiredCharacter.DialogueSprites.Add(new BixBite.Rendering.Sprite(eobj.Name, eobj.Thumbnail.AbsolutePath,0,0,0,0));
			if (DesiredCharacter.Name == null) DesiredCharacter.Name = CharacterName_TB.Text;
			if (AddToScene != null) AddToScene(DesiredCharacter);
			this.Close();
		}

	}
}
