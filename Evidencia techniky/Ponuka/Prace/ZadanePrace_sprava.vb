﻿Imports MySql.Data.MySqlClient
Imports Evidencia_techniky.pripojenie
Imports Evidencia_techniky.Zoznam_zadanych_prac
Public Class ZadanePrace_sprava

    Public Shared id_prace
    Public Shared PPracaCislo As String
    Dim PPrijatedna As String
    Dim POdovzdatDo As String
    Dim PSpracovane As String
    Dim PTypPrace As String
    Dim PStavPrace As String
    Dim PPopisPrace As String
    Dim PPriradene As String

    Public Sub ZadanePrace_sprava_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = hlavicka_programu(Me.Text, UCase(Ponuka.Meno_uzivatela))

        con.Open()
        Dim sqlquery As String =
        "SELECT p.ID_prace, p.Cislo_prace, date_format(p.Prijate, GET_FORMAT(DATE,'EUR')) as 'Datum zadania prace', date_format(p.Odovzdat_do, GET_FORMAT(DATE,'EUR')) as 'Odovzdat pracu do', date_format(p.Spracovane, GET_FORMAT(DATE,'EUR')) as 'Praca spracovana', cd2.Nazov_hodnoty as Typ_prace, cd4.Nazov_hodnoty as Stav_prace, p.Popis_prace, CONCAT_WS(' ', uz.Priezvisko, uz.meno) as 'Priradene'
        FROM prace p
        join ciselnik_data cd2 on p.Druh_prace = cd2.Hodnota and cd2.idciselnik = 9 and cd2.stav = 0
        join ciselnik_data cd4 on p.stav_prace = cd4.Hodnota and cd4.idciselnik = 11 and cd3.stav = 0
        join uzivatelia uz on p.id_uzivatela = uz.id_uzivatela
        WHERE p.id_prace = '" & Zoznam_zadanych_prac.id_prace & "' and p.stav = 0"
        Dim data As MySqlDataReader
        Dim adapter As New MySqlDataAdapter
        Dim command As New MySqlCommand
        command.CommandText = sqlquery
        command.Connection = con
        adapter.SelectCommand = command
        data = command.ExecuteReader
        Try
            If data.HasRows Then
                While data.Read()
                    id_prace = data("id_prace").ToString
                    PPracaCislo = data("Cislo_prace").ToString
                    PPrijatedna = data("Datum zadania prace").ToString
                    POdovzdatDo = data("Odovzdat pracu do").ToString
                    PSpracovane = data("Praca spracovana").ToString
                    PTypPrace = data("Typ_prace").ToString
                    PStavPrace = data("Stav_prace").ToString
                    PPopisPrace = data("Popis_prace").ToString
                    PPriradene = data("Priradene").ToString

                    l_PPracaCislo.Text = PPracaCislo
                    l_PDatumZadania.Text = PPrijatedna
                    cb_DruhPrace.Text = PTypPrace
                    dtp_OdovzdatDo.Text = POdovzdatDo
                    cb_StavPrace.Text = PStavPrace
                    tb_PopisPrace.Text = PPopisPrace
                    cb_Priradene.Text = PPriradene

                    If PSpracovane = "" Then
                        dtp_Spracovane.Visible = False
                        b_Spracovane.Visible = True
                    ElseIf PSpracovane <> "" Then
                        dtp_Spracovane.Text = PSpracovane
                    End If

                End While
                data.Close()
            End If
            con.Close()
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "ETECH - Vytiahnutie údajov ", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        'Vytiahnutie udajov z ciselnika -- TYP PRÁCE
        Dim QueryPrace As String
        QueryPrace =
        "SELECT cd.Nazov_hodnoty FROM uloha_x_uzivatel uxu
        join ciselnik_data cd on uxu.hodnota = cd.hodnota and uxu.idciselnik = cd.idciselnik and cd.stav = 0
        where uxu.id_uzivatela = '" & Ponuka.id_uzivatela & "' and uxu.idciselnik = 9 and uxu.stav = 0;"
        con.Open()
        Dim sqlPrace As MySqlCommand = New MySqlCommand(QueryPrace, con)
        Try
            Using odd As MySqlDataReader = sqlPrace.ExecuteReader()

                'Vytvorenie tabulky.
                Dim dtPrace As New DataTable("Prace")
                Dim ds2 As New DataSet()

                'Nacitanie dat
                dtPrace.Load(odd)

                'Pridanie dat do tabulky
                ds2.Tables.Add(dtPrace)

                Dim i As Integer = 0

                Do Until i = ds2.Tables(0).Rows.Count
                    cb_DruhPrace.Items.Add(ds2.Tables(0).Rows(i).Item(0))
                    i = i + 1
                Loop

            End Using
            con.Close()
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "ETECH - Zistenie typov prác", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        'Vytiahnutie udajov z ciselnika -- STAV ULOHY
        Dim QueryStavUloh As String
        QueryStavUloh =
        "SELECT cd.Nazov_hodnoty FROM uloha_x_uzivatel uxu
        join ciselnik_data cd on uxu.hodnota = cd.hodnota and uxu.idciselnik = cd.idciselnik and cd.stav = 0
        where uxu.id_uzivatela = '" & Ponuka.id_uzivatela & "' and uxu.idciselnik = 11 and uxu.Hodnota in (0, 1, 2, 5) and uxu.stav = 0;"
        con.Open()
        Dim sqlStavUloh As MySqlCommand = New MySqlCommand(QueryStavUloh, con)
        Try
            Using odd As MySqlDataReader = sqlStavUloh.ExecuteReader()

                'Vytvorenie tabulky.
                Dim dtStavUloh As New DataTable("StavUloh")
                Dim ds4 As New DataSet()

                'Nacitanie dat
                dtStavUloh.Load(odd)

                'Pridanie dat do tabulky
                ds4.Tables.Add(dtStavUloh)

                Dim i As Integer = 0

                Do Until i = ds4.Tables(0).Rows.Count
                    cb_StavPrace.Items.Add(ds4.Tables(0).Rows(i).Item(0))
                    i = i + 1
                Loop

            End Using
            con.Close()
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "ETECH - Zistenie stavov úloh", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try

        'Vytiahnutie pracovnikov na danu pracu
        Dim QueryPriradene As String
        QueryPriradene =
        "SELECT CONCAT_WS(' ', uz.Priezvisko, uz.Meno) FROM uzivatel_x_prace uxp
        join uzivatelia uz on uxp.id_uzivatela = uz.id_uzivatela and zablokovany = 0
        where uxp.id_prace = '" & ciselnik(1, 9, cb_DruhPrace.Text) & "' and uxp.id_uzivatela != '" & Ponuka.id_uzivatela & "' and uxp.stav = 0;"
        con.Open()
        Dim sqlPriradene As MySqlCommand = New MySqlCommand(QueryPriradene, con)
        Try
            Using odd As MySqlDataReader = sqlPriradene.ExecuteReader()

                'Vytvorenie tabulky.
                Dim dtPrace As New DataTable("Prace")
                Dim ds2 As New DataSet()

                'Nacitanie dat
                dtPrace.Load(odd)

                'Pridanie dat do tabulky
                ds2.Tables.Add(dtPrace)

                Dim i As Integer = 0

                Do Until i = ds2.Tables(0).Rows.Count
                    cb_Priradene.Items.Add(ds2.Tables(0).Rows(i).Item(0))
                    i = i + 1
                Loop

            End Using

            con.Close()
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "ETECH - Zistenie pracovnika prace", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End Try
    End Sub

    Private Sub b_Ulozit_Click(sender As Object, e As EventArgs) Handles b_Ulozit.Click

        Dim QueryPrace As String
        QueryPrace = "UPDATE prace SET Odovzdat_do = '" & uprava_datumu(dtp_OdovzdatDo.Text) & "', id_uzivatela = '" & uzivatel(cb_Priradene.Text) & "', Druh_prace = (select Hodnota from ciselnik_data where stav = 0 and idciselnik = 9 and CONVERT(Nazov_hodnoty USING utf8) = '" & cb_DruhPrace.Text & "'), Popis_prace = '" & tb_PopisPrace.Text & "', Stav_prace = (select Hodnota from ciselnik_data where stav = 0 and idciselnik = 11 and CONVERT(Nazov_hodnoty USING utf8) = '" & cb_StavPrace.Text & "'), Upravil_meno = '" & Ponuka.Meno_uzivatela & "', Upravil_dna = now() WHERE id_prace = '" & Zoznam_zadanych_prac.id_prace & "';"
        con.Open()
        Dim sqlPrace As MySqlCommand = New MySqlCommand(QueryPrace, con)
        Try
            sqlPrace.ExecuteNonQuery()
            con.Close()
            MessageBox.Show("Údaje BOLI upravené!", "ETECH - Zmena údajov v prácach", MessageBoxButtons.OK, MessageBoxIcon.Information)
            logy(11, 1, "")
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "Zmena údajov v prácach", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            logy(11, 2, ex.Message)
        End Try

        If PSpracovane <> "" Then
            Dim QuerySpracovane As String
            QuerySpracovane = "UPDATE prace SET Spracovane = '" & uprava_datumu(Now) & "', Upravil_dna = now() WHERE id_prace = '" & Zoznam_zadanych_prac.id_prace & "';"
            con.Open()
            Dim sqlSpracovane As MySqlCommand = New MySqlCommand(QuerySpracovane, con)
            Try
                sqlSpracovane.ExecuteNonQuery()
                con.Close()
            Catch ex As Exception
                con.Close()
                MessageBox.Show(ex.Message, "Zmena údajov v prácach", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                logy(11, 2, ex.Message)
            End Try
        End If

    End Sub

    Private Sub b_Spracovane_Click(sender As Object, e As EventArgs) Handles b_Spracovane.Click
        Dim QueryPrace As String
        QueryPrace = "UPDATE prace SET Spracovane = '" & uprava_datumu(Now) & "', Upravil_dna = now() WHERE id_prace = '" & Zoznam_zadanych_prac.id_prace & "';"
        con.Open()
        Dim sqlPrace As MySqlCommand = New MySqlCommand(QueryPrace, con)
        Try
            sqlPrace.ExecuteNonQuery()
            con.Close()
            MessageBox.Show("Práca BOLA spracovaná!", "ETECH - Zmena údajov v prácach", MessageBoxButtons.OK, MessageBoxIcon.Information)
            logy(11, 1, "")
        Catch ex As Exception
            con.Close()
            MessageBox.Show(ex.Message, "Zmena údajov v prácach", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            logy(11, 2, ex.Message)
        End Try

        b_Spracovane.Visible = False
        dtp_Spracovane.Visible = True
    End Sub
End Class