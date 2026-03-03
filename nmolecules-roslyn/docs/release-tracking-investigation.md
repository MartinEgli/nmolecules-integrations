# Release Tracking Investigation

Stand: 2026-03-03

## Problem

Das Analyzer-Projekt erzeugt mit aktivem Roslyn Release Tracking Warnungen `RS2002` oder `RS2003`, obwohl die Rule-IDs in den Analyzer-Klassen vorhanden sind.

Beobachteter Effekt:

- `AnalyzerReleases.Unshipped.md` fuehrt zu `RS2002`
- Verschiebung derselben Regeln nach `AnalyzerReleases.Shipped.md` fuehrt zu `RS2003`

Beide Varianten deuten darauf hin, dass ReleaseTrackingAnalyzers die bestehenden Diagnosen nicht korrekt den Analyzer-Typen zuordnen.

## Wahrscheinliche Ursache

Die Analyzer implementieren ihre Logik ueber einen generischen Basistyp:

- `Analyzer<TAttribute> : DiagnosticAnalyzer`

Moegliche Folge:

- ReleaseTrackingAnalyzers erkennen die konkreten `SupportedDiagnostics` der abgeleiteten Klassen nicht stabil
- dadurch wirken alle Rule-IDs aus Sicht des Trackings wie "nicht mehr unterstuetzt"

Das passt zum aktuellen Symptom:

- nicht einzelne Rules sind betroffen
- sondern saemtliche bestehenden Diagnose-IDs

## Aktueller Umgang

Im Projekt wird `RS2002` und `RS2003` temporaer unterdrueckt, damit:

- Builds lesbar bleiben
- neue fachliche Regeln weiter entwickelt werden koennen
- die Release-Dateien trotzdem als inhaltliche Basis gepflegt werden

## Saubere Langfrist-Loesungen

### Option 1: Analyzer-Struktur refaktorieren

- konkrete Analyzer direkt von `DiagnosticAnalyzer` ableiten lassen
- generische Basisklasse entfernen oder auf Hilfsklassen reduzieren

Vorteil:

- beste Chance auf native Kompatibilitaet mit ReleaseTrackingAnalyzers

Nachteil:

- groessere Umstellung der Analyzer-Struktur

### Option 2: Release-Tracking-Ansatz anpassen

- ReleaseTrackingAnalyzers nicht als harte Build-Warnung verwenden
- Rule-Inventar ueber eigene Dokumentation und Tests absichern

Vorteil:

- wenig struktureller Umbau

Nachteil:

- weniger automatische Sicherung der Release-Historie

## Empfohlener naechster Schritt

1. fachliche Arbeit an neuen Regeln fortsetzen
2. parallel einen kleinen Spike planen:
   einen Analyzer probeweise ohne generischen Basistyp implementieren und pruefen, ob `RS2002/RS2003` verschwindet
