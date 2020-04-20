USE [StratenSegmenten];

CREATE TABLE Punt(
	Id int IDENTITY (1,1) NOT NULL,
	X decimal(15,9),
	Y decimal(17,11),
	PRIMARY KEY (Id)
	);

CREATE TABLE Knoop(
	Id int NOT NULL,
	PuntId int,
	PRIMARY KEY (Id)
); 


CREATE TABLE Segment(
Id int not null,
BeginKnoopId int,
EindKnoopId int,
PRIMARY KEY(Id),
);

CREATE TABLE Graaf(
Id int not null,
PRIMARY KEY (Id)
);


CREATE TABLE Straat
(Id int NOT NULL,
StraatNaam NVARCHAR(500),
GraafId int,
GemeenteId int,
PRIMARY KEY (Id)
);

CREATE TABLE Gemeente
(Id int not null,
NaamId int,
TaalCode NVARCHAR(50),
Naam NVARCHAR(500),
ProvincieId int,
PRIMARY KEY (Id)
);

CREATE TABLE Provincie
(Id int not null,
TaalCode NVARCHAR(50),
Naam NVARCHAR(500),
RegioId int,
PRIMARY KEY (Id)
);

CREATE TABLE Regio
(Id int not null,
Naam NVARCHAR(500),
LandId int,
PRIMARY KEY(Id)
);

CREATE TABLE Land
(Id int not null,
Naam NVARCHAR (500),
TaalCode NVARCHAR (50),
PRIMARY KEY (Id)
);


CREATE TABLE GraafId_SegmentId(
GraafId int not null,
SegmentId int not null
);
ALTER TABLE GraafId_SegmentId
ADD PRIMARY KEY (GraafId, SegmentId);

CREATE TABLE SegmentId_PuntId(
SegmentId int not null,
PuntId int not null,
);
ALTER TABLE SegmentId_PuntId
ADD PRIMARY KEY (SegmentId, PuntId);



ALTER TABLE Straat 
ADD CONSTRAINT fk_gemId_straat FOREIGN KEY (GemeenteId) REFERENCES Gemeente(Id);
ALTER TABLE Gemeente 
ADD CONSTRAINT fk_proId_gem FOREIGN KEY (ProvincieId) REFERENCES Provincie(Id);
ALTER TABLE Provincie 
ADD CONSTRAINT fk_reigId_pro FOREIGN KEY (RegioId) REFERENCES Regio(Id);
ALTER TABLE Regio 
ADD CONSTRAINT fk_landId_reg FOREIGN KEY (LandId) REFERENCES Land(Id);
ALTER TABLE Knoop
ADD CONSTRAINT fk_puntId_Knoop FOREIGN KEY(PuntId) references Punt (Id);
ALTER TABLE Segment
ADD CONSTRAINT fk_beginknoopId_Segment FOREIGN KEY(BeginKnoopId) references Knoop (Id);
ALTER TABLE Segment
ADD CONSTRAINT fk_eindknoopId_Segment FOREIGN KEY(EindKnoopId) references Knoop (Id);
ALTER TABLE SegmentId_PuntId
ADD CONSTRAINT fk_puntID_segmentId_PuntId FOREIGN KEY (PuntId) REFERENCES Punt (Id);
ALTER TABLE Straat
ADD CONSTRAINT fk_graafId_Straat FOREIGN KEY(GraafId) REFERENCES Graaf(Id);


