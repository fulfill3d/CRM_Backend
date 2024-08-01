ALTER TABLE StoreLocations ADD Latitude decimal(9, 6) NOT NULL DEFAULT 0;
ALTER TABLE StoreLocations ADD Longitude decimal(9, 6) NOT NULL DEFAULT 0;
ALTER TABLE StoreLocations DROP COLUMN Location;
