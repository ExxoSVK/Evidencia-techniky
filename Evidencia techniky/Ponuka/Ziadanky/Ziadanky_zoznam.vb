﻿Imports MySql.Data.MySqlClient
Imports Evidencia_techniky.Ponuka
Imports Evidencia_techniky.pripojenie

Public Class Ziadanky_zoznam
    Public Shared Poziadavka_cislo As String
    Private Sub Ziadanky_sprava_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = hlavicka_programu(Me.Text, Ponuka.Meno_uzivatela)
    End Sub

    Public Sub b_NacitatData_Click(sender As Object, e As EventArgs) Handles b_NacitatData.Click
        ProgressBar1.Maximum = 0
        Spracovanie_dat.Show()
        Threading.Thread.Sleep(100)

        'Vyčistenie vyslednej tabuliek
        dgv_Nove.Rows.Clear()
        dgv_Rozpracovane.Rows.Clear()
        dgv_Odlozene.Rows.Clear()
        dgv_VrateneZadavatelovi.Rows.Clear()
        dgv_VrateneUdrzbe.Rows.Clear()
        dgv_UkonceneUdrzba.Rows.Clear()
        dgv_Investicie.Rows.Clear()

        'Data pre naplnenie tabulky NOVE
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ' 
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenia.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and Stav_ulohy = 0 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_Nove.ColumnCount = 7
                dgv_Nove.Columns(0).Name = "Úloha číslo"
                dgv_Nove.Columns(1).Name = "Typ požiadavky"
                dgv_Nove.Columns(2).Name = "Typ práce"
                dgv_Nove.Columns(3).Name = "Dátum"
                dgv_Nove.Columns(4).Name = "Urgencia"
                dgv_Nove.Columns(5).Name = "Oddelenie"
                dgv_Nove.Columns(6).Name = "Zadávateľ"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_Nove.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop
            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky ROZPRACOVANE
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ',
                '|' as '|',
                p.Cislo_prace as 'Číslo práce',
				date_format(p.Prijate, GET_FORMAT(DATE,'EUR')) as 'Zadané', 
                CONCAT_WS(' ', up.meno, up.Priezvisko) as 'Priradené'
            From uloha u
            left join uloha_x_prace uxp on u.id_ulohy = uxp.id_uloha and uxp.stav = 0
            left join prace p on uxp.id_prace = p.id_prace and p.stav = 0
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziaadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenia.stav = 0 and oddelenia.stav = 0
            left join uzivatelia up on p.id_uzivatela = up.id_uzivatela
            left join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and Stav_ulohy = 1 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            order by 11 desc
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_Rozpracovane.ColumnCount = 11
                dgv_Rozpracovane.Columns(0).Name = "Úloha číslo"
                dgv_Rozpracovane.Columns(1).Name = "Typ požiadavky"
                dgv_Rozpracovane.Columns(2).Name = "Typ práce"
                dgv_Rozpracovane.Columns(3).Name = "Dátum"
                dgv_Rozpracovane.Columns(4).Name = "Urgencia"
                dgv_Rozpracovane.Columns(5).Name = "Oddelenie"
                dgv_Rozpracovane.Columns(6).Name = "Zadávateľ"
                dgv_Rozpracovane.Columns(7).Name = "|"
                dgv_Rozpracovane.Columns(8).Name = "Číslo práce"
                dgv_Rozpracovane.Columns(9).Name = "Zadané"
                dgv_Rozpracovane.Columns(10).Name = "Priradené"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_Rozpracovane.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString, ds.Tables(0).Rows(i).Item(7).ToString, ds.Tables(0).Rows(i).Item(8).ToString, ds.Tables(0).Rows(i).Item(9).ToString, ds.Tables(0).Rows(i).Item(10).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky ODLOZENE
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ',
                (select dovod_stav from uloha_x_stav uxs where u.id_ulohy = uxs.id_ulohy and stav = 0 order by 1 desc limit 1) as 'Stav'
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenia.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and Stav_ulohy = 2 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_Odlozene.ColumnCount = 8
                dgv_Odlozene.Columns(0).Name = "Úloha číslo"
                dgv_Odlozene.Columns(1).Name = "Typ požiadavky"
                dgv_Odlozene.Columns(2).Name = "Typ práce"
                dgv_Odlozene.Columns(3).Name = "Dátum"
                dgv_Odlozene.Columns(4).Name = "Urgencia"
                dgv_Odlozene.Columns(5).Name = "Oddelenie"
                dgv_Odlozene.Columns(6).Name = "Zadávateľ"
                dgv_Odlozene.Columns(7).Name = "Stav"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_Odlozene.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString, ds.Tables(0).Rows(i).Item(7).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky VRATENA ZADAVATELA
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ',
                (select dovod_stav from uloha_x_stav uxs where u.id_ulohy = uxs.id_ulohy and stav = 0 order by 1 desc limit 1) as 'Dôvod vrátenia'
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenia.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and u.Stav_ulohy = 3 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_VrateneZadavatelovi.ColumnCount = 8
                dgv_VrateneZadavatelovi.Columns(0).Name = "Úloha číslo"
                dgv_VrateneZadavatelovi.Columns(1).Name = "Typ požiadavky"
                dgv_VrateneZadavatelovi.Columns(2).Name = "Typ práce"
                dgv_VrateneZadavatelovi.Columns(3).Name = "Dátum"
                dgv_VrateneZadavatelovi.Columns(4).Name = "Urgencia"
                dgv_VrateneZadavatelovi.Columns(5).Name = "Oddelenie"
                dgv_VrateneZadavatelovi.Columns(6).Name = "Zadávateľ"
                dgv_VrateneZadavatelovi.Columns(7).Name = "Dôvod vrátenia"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_VrateneZadavatelovi.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString, ds.Tables(0).Rows(i).Item(7).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky VRATENE UDRZBA
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ' 
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenia.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and Stav_ulohy = 4 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_VrateneUdrzbe.ColumnCount = 7
                dgv_VrateneUdrzbe.Columns(0).Name = "Úloha číslo"
                dgv_VrateneUdrzbe.Columns(1).Name = "Typ požiadavky"
                dgv_VrateneUdrzbe.Columns(2).Name = "Typ práce"
                dgv_VrateneUdrzbe.Columns(3).Name = "Dátum"
                dgv_VrateneUdrzbe.Columns(4).Name = "Urgencia"
                dgv_VrateneUdrzbe.Columns(5).Name = "Oddelenie"
                dgv_VrateneUdrzbe.Columns(6).Name = "Zadávateľ"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_VrateneUdrzbe.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky UKONCENE UDRZBA
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ' 
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenie.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 0 and Stav_ulohy = 5 and u.stav = 0 and (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_UkonceneUdrzba.ColumnCount = 7
                dgv_UkonceneUdrzba.Columns(0).Name = "Úloha číslo"
                dgv_UkonceneUdrzba.Columns(1).Name = "Typ požiadavky"
                dgv_UkonceneUdrzba.Columns(2).Name = "Typ práce"
                dgv_UkonceneUdrzba.Columns(3).Name = "Dátum"
                dgv_UkonceneUdrzba.Columns(4).Name = "Urgencia"
                dgv_UkonceneUdrzba.Columns(5).Name = "Oddelenie"
                dgv_UkonceneUdrzba.Columns(6).Name = "Zadávateľ"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_UkonceneUdrzba.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Data pre naplnenie tabulky INVESTICIA
        Using cmd As New MySqlCommand("Select
	            Uloha_cislo as 'Žiadanka číslo',
                cd_poziadavka.Nazov_hodnoty as 'Typ požiadavky', 
	            cd_prace.Nazov_hodnoty as 'Typ práce', 
	            date_format(u.Nahlasene_dna, GET_FORMAT(DATE,'EUR')) as 'Dátum', 
                case 
	            when Urgencia = 0 then 'Nie' 
	            Else 'Áno' 
                        End As 'Urgencia', 
	            oddelenia.Nazov_oddelenia as 'Oddelenie', 
	            CONCAT_WS(' ', uzivatelia.meno, uzivatelia.Priezvisko) as 'Zadávateľ' 
            From uloha u
            Join ciselnik_data cd_poziadavka on u.Typ_poziadavky = cd_poziadavka.Hodnota And cd_poziadavka.idciselnik = 8 and cd_poziadavka.stav = 0
            Join ciselnik_data cd_prace on u.Typ_prace = cd_prace.Hodnota And cd_prace.idciselnik = 9 and cd_prace.stav = 0
            Join oddelenia On u.oddelenie = oddelenia.id_oddelenia and oddelenie.stav = 0
            Join uzivatelia On u.Nahlasil_ID_zamestanca = uzivatelia.id_uzivatela
            WHERE
            Typ_ulohy = 1 and u.stav = 0 (u.Nahlasil_ID_zamestanca = '" & Ponuka.id_uzivatela & "' or '" & Ponuka.id_uzivatela & "' IN (7, 2))
            ;")
            cmd.Connection = con
            cmd.CommandTimeout = 1200
            con.Open()
            Using sdr As MySqlDataReader = cmd.ExecuteReader()

                'Vytvorenie tabulky
                Dim dtCustomers As New DataTable("Poziadavky")
                Dim ds As New DataSet()

                'Nacitanie dat
                dtCustomers.Load(sdr)

                'Pridanie dat do tabulky
                ds.Tables.Add(dtCustomers)

                dgv_Investicie.ColumnCount = 7
                dgv_Investicie.Columns(0).Name = "Úloha číslo"
                dgv_Investicie.Columns(1).Name = "Typ požiadavky"
                dgv_Investicie.Columns(2).Name = "Typ práce"
                dgv_Investicie.Columns(3).Name = "Dátum"
                dgv_Investicie.Columns(4).Name = "Urgencia"
                dgv_Investicie.Columns(5).Name = "Oddelenie"
                dgv_Investicie.Columns(6).Name = "Zadávateľ"

                ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                ProgressBar1.Value = ProgressBar1.Value + 1

                Dim i As Integer = 0

                Do Until i = ds.Tables(0).Rows.Count
                    dgv_Investicie.Rows.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString, ds.Tables(0).Rows(i).Item(2).ToString, ds.Tables(0).Rows(i).Item(3).ToString, ds.Tables(0).Rows(i).Item(4).ToString, ds.Tables(0).Rows(i).Item(5).ToString, ds.Tables(0).Rows(i).Item(6).ToString)
                    ProgressBar1.Maximum = ProgressBar1.Maximum + 1
                    ProgressBar1.Value = ProgressBar1.Value + 1
                    i = i + 1
                Loop

            End Using
            Spracovanie_dat.Close()
            con.Close()
        End Using
        con.Close()

        'Nastavenia velkosti stlpcov
        dgv_Nove.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_Rozpracovane.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_Odlozene.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_VrateneUdrzbe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_VrateneZadavatelovi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_UkonceneUdrzba.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv_Investicie.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        ProgressBar1.Maximum = ProgressBar1.Maximum + 1
        ProgressBar1.Value = ProgressBar1.Value + 1

    End Sub

    Public Sub selectedRowsButton_Click_Nove(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_Nove.Click
        Poziadavka_cislo = dgv_Nove.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_Rozpracovane(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_Rozpracovane.Click
        Poziadavka_cislo = dgv_Rozpracovane.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_Odlozene(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_Odlozene.Click
        Poziadavka_cislo = dgv_Odlozene.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_VrateneZadavatelovi(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_VrateneZadavatelovi.Click
        Poziadavka_cislo = dgv_VrateneZadavatelovi.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_VrateneUdrzba(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_VrateneUdrzbe.Click
        Poziadavka_cislo = dgv_VrateneUdrzbe.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_UkonceneUdrzba(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_UkonceneUdrzba.Click
        Poziadavka_cislo = dgv_VrateneUdrzbe.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

    Public Sub selectedRowsButton_Click_Investicie(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgv_Investicie.Click
        Poziadavka_cislo = dgv_Investicie.CurrentRow.Cells("Úloha číslo").Value
        Ziadanky_sprava.Show()
    End Sub

End Class