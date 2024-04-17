CREATE DATABASE Mini_Project;

USE Mini_Project;

CREATE TABLE Train_Book (
	Sl_No Numeric(20) Primary Key,
    Train_No NVARCHAR(100),
    Train_Name NVARCHAR(100),
    Arrival_Time NVARCHAR(50),
    Departure_Time NVARCHAR(50),
    Travel_Duration NVARCHAR(50),
    Class NVARCHAR(50),
    Total_Berth INT,
    Available_Berth INT,
    [From] NVARCHAR(100),
    [To] NVARCHAR(100),
    Runs_On NVARCHAR(100),
    Price numeric(20,2) 
);

INSERT INTO Train_Book (Sl_No, Train_No, Train_Name, Arrival_Time, Departure_Time, Travel_Duration,  Class, Total_Berth, Available_Berth, [From], [To], Runs_On, Price) 
VALUES 
(1, '20653', 'SBC BGM EXP', '21:00', '06:45', '09:45', 'AC First Class (1A)', 70, 70, 'Bengaluru', 'Belagavi', 'M T W T F S S', 2335),
(2, '20653', 'SBC BGM EXP', '21:00', '06:45', '09:45', 'AC 2 Tier (2A)', 90, 90, 'Bengaluru', 'Belagavi', 'M T W T F S S', 1400),
(3, '20653', 'SBC BGM EXP', '21:00', '06:45', '09:45', 'AC 3 Tier (3A)', 100, 100, 'Bengaluru', 'Belagavi', 'M T W T F S S', 995),
(4, '20653', 'SBC BGM EXP', '21:00', '06:45', '09:45', 'AC 3 Economy (3E)', 150, 150, 'Bengaluru', 'Belagavi', 'M T W T F S S', 920),
(5, '20653', 'SBC BGM EXP', '21:00', '06:45', '09:45', 'Sleeper (SL)', 350, 350, 'Bengaluru', 'Belagavi', 'M T W T F S S', 380),
----------------------------------------------------------
(6, '20654', 'BGM SBC EXP', '21:00', '07:30', '10:30', 'AC First Class (1A)', 60, 60, 'Belagavi', 'Bengaluru', 'M T W T F S S', 2335),
(7, '20654', 'BGM SBC EXP', '21:00', '07:30', '10:30', 'AC 2 Tier (2A)', 80, 80, 'Belagavi', 'Bengaluru', 'M T W T F S S', 1400),
(8, '20654', 'BGM SBC EXP', '21:00', '07:30', '10:30', 'AC 3 Tier (3A)', 90, 90, 'Belagavi', 'Bengaluru', 'M T W T F S S', 995),
(9, '20654', 'BGM SBC EXP', '21:00', '07:30', '10:30', 'AC 3 Economy (3E)', 150, 150, 'Belagavi', 'Bengaluru', 'M T W T F S S', 920),
(10, '20654', 'BGM SBC EXP', '21:00', '07:30', '10:30', 'Sleeper (SL)', 300, 300, 'Belagavi', 'Bengaluru', 'M T W T F S S', 380),
------------------------------------------------------------------

(11, '20661', 'DWR VANDE BHARAT', '05:45', '11:00', '05:15', 'Exec. Chair Car (EC)', 50, 50, 'Bengaluru', 'SSS HUBBALLI JN', 'M t W T F S S', 2200),
(12, '20661', 'DWR VANDE BHARAT', '05:45', '11:00', '05:15', 'AC Chair car (CC)', 400, 400, 'Bengaluru', 'SSS HUBBALLI JN', 'M t W T F S S', 1155),
------------------------------------------------------------------
(13, '20662', 'SBC VANDE BHARAT', '13:40', '19:45', '06:05', 'Exec. Chair Car (EC)', 60, 60, 'SSS HUBBALLI JN', 'Bengaluru', 'M t W T F S S', 2200),
(14, '20662', 'SBC VANDE BHARAT', '13:40', '19:45', '06:05', 'AC Chair car (CC)', 450, 450, 'SSS HUBBALLI JN', 'Bengaluru', 'M t W T F S S', 1155);
------------------------------------------------------------------

ALTER TABLE Train_Book
ADD IsActive BIT NOT NULL DEFAULT 1; -- Set the default value to 1 (True)


SELECT * FROM Train_Book;

CREATE TABLE Train_Status (
	Sl_No numeric(5) primary key,
    Train_No NVARCHAR(100),
    IsActive BIT
);

ALTER TABLE Train_Status
ADD IsActive BIT;


select * from Train_Status

Drop table Train_Status



-----------------------------------------------------------------------------------------------------------
CREATE TABLE Customers (
    Username NVARCHAR(100) PRIMARY KEY,
    Password NVARCHAR(100)
);

------------------------------------------------------------------------

CREATE TABLE BookedTickets (
    BookingID NVARCHAR(100), 
    TrainNumber NVARCHAR(100),
    Class NVARCHAR(100),
    PassengerName VARCHAR(15),
    PassengerAge NUMERIC(3),
    PassengerGender VARCHAR(5),
	IsCanceled NVARCHAR(100)
);

-- Check if a booking exists for a given BookingID and username
SELECT COUNT(*) FROM BookedTickets WHERE BookingID = BookingID AND Username = Username AND IsCanceled = '0';

-- Update the cancellation status for a given BookingID
UPDATE BookedTickets SET IsCanceled = '0' WHERE BookingID = BookingID;

SELECT * FROM BookedTickets;

drop table BookedTickets
Select * from Customers;
drop table Customers
