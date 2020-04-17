
-- Get al the StraatIds from 1 gemeente
SELECT s.Id 
FROM Straat s
JOIN Gemeente g
ON (s.GemeenteId = g.Id)
WHERE s.GemeenteId = (SELECT Id FROM Gemeente WHERE Naam = 'Lievegem');

-- Get StraatInfo with gemeenteNaam from straatId
SELECT StraatNaam, GraafId, GemeenteId
FROM Straat s 
WHERE s.Id = 5;

-- Get all segmenten from graafId
SELECT s.Id,s.BeginKnoopId, s.EindKnoopId
FROM Segment s JOIN GraafId_SegmentId gs
ON (s.Id = gs.SegmentId)
WHERE gs.GraafId = 5;

-- Get all punten of a segment
SELECT X, Y
FROM Punt p JOIN SegmentId_PuntId sp
ON (p.Id = sp.PuntId)
WHERE sp.SegmentId = 455006
ORDER BY p.Id;

--Get punt of KnoopId
SELECT p.X,p.Y
FROM punt p JOIN Knoop k 
ON (k.PuntId = p.Id)
WHERE k.Id = 4562;

--Get straatId from straatnaam and gemeenteNaam
SELECT s.Id
FROM Straat s JOIN Gemeente g
ON (s.GemeenteId = g.Id)
WHERE s.StraatNaam = 'Acacialaan' AND g.Naam = 'Destelbergen';

--Get gemeenteNaam en ProvincieNaam by GemeenteId
SELECT g.Naam, p.Naam
FROM Gemeente g JOIN Provincie p
ON (g.ProvincieId = p.Id)
WHERE g.Id = 5;

--Get GemeenteId/Naam by ProvincieId
SELECT Id, Naam
FROM Gemeente
WHERE ProvincieId = 1;

--GetKnopen for straatId
(SELECT s.BeginKnoopId
FROM Segment s JOIN GraafId_SegmentId gs
ON(s.Id = gs.SegmentId)
WHERE gs.GraafId = (SELECT GraafId
FROM Straat
WHERE Id = 456))
UNION
(SELECT s.EindKnoopId
FROM Segment s JOIN GraafId_SegmentId gs
ON(s.Id = gs.SegmentId)
WHERE gs.GraafId = (SELECT GraafId
FROM Straat
WHERE Id = 456));