
// This file has been generated by the GUI designer. Do not modify.
namespace Apperian.Ease.Publisher
{
	public partial class PublisherDialog
	{
		private global::Gtk.Table table1;
		private global::Gtk.Label authorEntry;
		private global::Gtk.Entry descriptionEntry;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView versionNotesEntry;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TextView lonDescriptionEntry;
		private global::Gtk.HBox hbox1;
		private global::Gtk.ComboBox comboboxTargets;
		private global::Gtk.Button buttonRegister;
		private global::Gtk.HSeparator hseparator1;
		private global::Gtk.HSeparator hseparator2;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label4;
		private global::Gtk.Label label5;
		private global::Gtk.Label label7;
		private global::Gtk.Label label8;
		private global::Gtk.Label nameEntry;
		private global::Gtk.Label versionEntry;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonPublish;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Apperian.Ease.Publisher.PublisherDialog
			this.Name = "Apperian.Ease.Publisher.PublisherDialog";
			this.Title = global::Mono.Unix.Catalog.GetString ("Publish to Apperian EASE");
			this.Icon = global::Gdk.Pixbuf.LoadFromResource ("Apperian.Ease.Publisher.icons.ease.png");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.Modal = true;
			this.Resizable = false;
			this.DestroyWithParent = true;
			// Internal child Apperian.Ease.Publisher.PublisherDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(9)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.BorderWidth = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.authorEntry = new global::Gtk.Label ();
			this.authorEntry.Name = "authorEntry";
			this.authorEntry.Xalign = 0F;
			this.authorEntry.LabelProp = global::Mono.Unix.Catalog.GetString ("...");
			this.table1.Add (this.authorEntry);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.authorEntry]));
			w2.TopAttach = ((uint)(3));
			w2.BottomAttach = ((uint)(4));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.descriptionEntry = new global::Gtk.Entry ();
			this.descriptionEntry.CanFocus = true;
			this.descriptionEntry.Name = "descriptionEntry";
			this.descriptionEntry.IsEditable = true;
			this.descriptionEntry.MaxLength = 100;
			this.descriptionEntry.InvisibleChar = '●';
			this.table1.Add (this.descriptionEntry);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.descriptionEntry]));
			w3.TopAttach = ((uint)(6));
			w3.BottomAttach = ((uint)(7));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.versionNotesEntry = new global::Gtk.TextView ();
			this.versionNotesEntry.WidthRequest = 450;
			this.versionNotesEntry.HeightRequest = 150;
			this.versionNotesEntry.CanFocus = true;
			this.versionNotesEntry.Name = "versionNotesEntry";
			this.versionNotesEntry.AcceptsTab = false;
			this.GtkScrolledWindow.Add (this.versionNotesEntry);
			this.table1.Add (this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow]));
			w5.TopAttach = ((uint)(8));
			w5.BottomAttach = ((uint)(9));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.lonDescriptionEntry = new global::Gtk.TextView ();
			this.lonDescriptionEntry.CanFocus = true;
			this.lonDescriptionEntry.Name = "lonDescriptionEntry";
			this.lonDescriptionEntry.AcceptsTab = false;
			this.GtkScrolledWindow1.Add (this.lonDescriptionEntry);
			this.table1.Add (this.GtkScrolledWindow1);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow1]));
			w7.TopAttach = ((uint)(7));
			w7.BottomAttach = ((uint)(8));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.comboboxTargets = global::Gtk.ComboBox.NewText ();
			this.comboboxTargets.Name = "comboboxTargets";
			this.hbox1.Add (this.comboboxTargets);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.comboboxTargets]));
			w8.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonRegister = new global::Gtk.Button ();
			this.buttonRegister.CanFocus = true;
			this.buttonRegister.Name = "buttonRegister";
			this.buttonRegister.UseUnderline = true;
			this.buttonRegister.Label = global::Mono.Unix.Catalog.GetString ("Register Publish Target...");
			this.hbox1.Add (this.buttonRegister);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonRegister]));
			w9.Position = 1;
			w9.Expand = false;
			w9.Fill = false;
			this.table1.Add (this.hbox1);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.hbox1]));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.table1.Add (this.hseparator1);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.hseparator1]));
			w11.TopAttach = ((uint)(5));
			w11.BottomAttach = ((uint)(6));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.table1.Add (this.hseparator2);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.hseparator2]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.RightAttach = ((uint)(2));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Target:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Description:");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w14.TopAttach = ((uint)(6));
			w14.BottomAttach = ((uint)(7));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Ypad = 5;
			this.label3.Xalign = 0F;
			this.label3.Yalign = 0F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Release Notes:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w15.TopAttach = ((uint)(8));
			w15.BottomAttach = ((uint)(9));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Name:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w16.TopAttach = ((uint)(2));
			w16.BottomAttach = ((uint)(3));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Ypad = 5;
			this.label5.Xalign = 0F;
			this.label5.Yalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Long Description:");
			this.table1.Add (this.label5);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table1 [this.label5]));
			w17.TopAttach = ((uint)(7));
			w17.BottomAttach = ((uint)(8));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 0F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Author:");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w18.TopAttach = ((uint)(3));
			w18.BottomAttach = ((uint)(4));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 0F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Version:");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w19.TopAttach = ((uint)(4));
			w19.BottomAttach = ((uint)(5));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.nameEntry = new global::Gtk.Label ();
			this.nameEntry.Name = "nameEntry";
			this.nameEntry.Xalign = 0F;
			this.nameEntry.LabelProp = global::Mono.Unix.Catalog.GetString ("...");
			this.table1.Add (this.nameEntry);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table1 [this.nameEntry]));
			w20.TopAttach = ((uint)(2));
			w20.BottomAttach = ((uint)(3));
			w20.LeftAttach = ((uint)(1));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.versionEntry = new global::Gtk.Label ();
			this.versionEntry.Name = "versionEntry";
			this.versionEntry.Xalign = 0F;
			this.versionEntry.LabelProp = global::Mono.Unix.Catalog.GetString ("...");
			this.table1.Add (this.versionEntry);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table1 [this.versionEntry]));
			w21.TopAttach = ((uint)(4));
			w21.BottomAttach = ((uint)(5));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.table1);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(w1 [this.table1]));
			w22.Position = 0;
			w22.Expand = false;
			w22.Fill = false;
			// Internal child Apperian.Ease.Publisher.PublisherDialog.ActionArea
			global::Gtk.HButtonBox w23 = this.ActionArea;
			w23.Name = "dialog1_ActionArea";
			w23.Spacing = 10;
			w23.BorderWidth = ((uint)(5));
			w23.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w24 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w23 [this.buttonCancel]));
			w24.Expand = false;
			w24.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonPublish = new global::Gtk.Button ();
			this.buttonPublish.CanDefault = true;
			this.buttonPublish.CanFocus = true;
			this.buttonPublish.Name = "buttonPublish";
			this.buttonPublish.Label = global::Mono.Unix.Catalog.GetString ("Publish");
			this.AddActionWidget (this.buttonPublish, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w25 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w23 [this.buttonPublish]));
			w25.Position = 1;
			w25.Expand = false;
			w25.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 592;
			this.DefaultHeight = 445;
			this.Show ();
		}
	}
}
