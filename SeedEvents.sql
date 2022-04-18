DELETE FROM [RunningEvents];
DELETE FROM [RunningRelayEvents];
DELETE FROM [FieldEvents];
DELETE FROM [FieldRelayEvents];

INSERT INTO
    [RunningEvents]
        ([Id], [Name], [Gender], [SortOrder], [DateCreated], [Environment], [Deleted])
    VALUES
        (NEWID(), '60M', 1, 1, SYSDATETIME(), 1, 0),
        (NEWID(), '200M', 1, 2, SYSDATETIME(), 1, 0),
        (NEWID(), '400M', 1, 3, SYSDATETIME(), 1, 0),
        (NEWID(), '800M', 1, 4, SYSDATETIME(), 1, 0),
        (NEWID(), '1600M', 1, 5, SYSDATETIME(), 1, 0),
        (NEWID(), '3200M', 1, 6, SYSDATETIME(), 1, 0),
        (NEWID(), '60M Hurdles', 1, 7, SYSDATETIME(), 1, 0),
           
        (NEWID(), '100M', 1, 1, SYSDATETIME(), 2, 0),
        (NEWID(), '200M', 1, 2, SYSDATETIME(), 2, 0),
        (NEWID(), '400M', 1, 3, SYSDATETIME(), 2, 0),
        (NEWID(), '800M', 1, 4, SYSDATETIME(), 2, 0),
        (NEWID(), '1600M', 1, 5, SYSDATETIME(), 2, 0),
        (NEWID(), '3200M', 1, 6, SYSDATETIME(), 2, 0),
        (NEWID(), '110M Hurdles', 1, 7, SYSDATETIME(), 2, 0),
        (NEWID(), '300M Hurdles', 1, 8, SYSDATETIME(), 2, 0),
        (NEWID(), '2K Steeple Chase', 1, 9, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 100M', 1, 10, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 200M', 1, 11, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 400M', 1, 12, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 800M', 1, 13, SYSDATETIME(), 2, 0),
        
        (NEWID(), '60M', 2, 1, SYSDATETIME(), 1, 0),
        (NEWID(), '200M', 2, 2, SYSDATETIME(), 1, 0),
        (NEWID(), '400M', 2, 3, SYSDATETIME(), 1, 0),
        (NEWID(), '800M', 2, 4, SYSDATETIME(), 1, 0),
        (NEWID(), '1600M', 2, 5, SYSDATETIME(), 1, 0),
        (NEWID(), '3200M', 2, 6, SYSDATETIME(), 1, 0),
        (NEWID(), '60M Hurdles', 2, 7, SYSDATETIME(), 1, 0),

        (NEWID(), '100M', 2, 1, SYSDATETIME(), 2, 0),
        (NEWID(), '200M', 2, 2, SYSDATETIME(), 2, 0),
        (NEWID(), '400M', 2, 3, SYSDATETIME(), 2, 0),
        (NEWID(), '800M', 2, 4, SYSDATETIME(), 2, 0),
        (NEWID(), '1600M', 2, 5, SYSDATETIME(), 2, 0),
        (NEWID(), '3200M', 2, 6, SYSDATETIME(), 2, 0),
        (NEWID(), '110M Hurdles', 2, 7, SYSDATETIME(), 2, 0),
        (NEWID(), '300M Hurdles', 2, 8, SYSDATETIME(), 2, 0),
        (NEWID(), '2K Steeple Chase', 2, 9, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 100M', 2, 10, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 200M', 2, 11, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 400M', 2, 12, SYSDATETIME(), 2, 0),
        (NEWID(), 'Seated 800M', 2, 13, SYSDATETIME(), 2, 0);

INSERT INTO
    [RunningRelayEvents]
([Id], [Name], [Gender], [SortOrder], [DateCreated], [Environment], [Deleted])
VALUES
    (NEWID(), '4x200M Relay', 1, 1, SYSDATETIME(), 1, 0),
    (NEWID(), '4x400M Relay', 1, 2, SYSDATETIME(), 1, 0),
    (NEWID(), '4x800M Relay', 1, 3, SYSDATETIME(), 1, 0),

    (NEWID(), '4x100M Relay', 1, 1, SYSDATETIME(), 2, 0),
    (NEWID(), '4x200M Relay', 1, 2, SYSDATETIME(), 2, 0),
    (NEWID(), '4x400M Relay', 1, 3, SYSDATETIME(), 2, 0),
    (NEWID(), '4x800M Relay', 1, 4, SYSDATETIME(), 2, 0),
    (NEWID(), '4x1600M Relay', 1, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Sprint Medley', 1, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Distance Medley', 1, 7, SYSDATETIME(), 2, 0),
    (NEWID(), '4x110M Shuttle Hurdles', 1, 8, SYSDATETIME(), 2, 0),
    (NEWID(), '4x300M Shuttle Hurdles', 1, 9, SYSDATETIME(), 2, 0),
    (NEWID(), '4x100M Thrower Relay', 1, 10, SYSDATETIME(), 2, 0),
       
    (NEWID(), '4x200M Relay', 2, 1, SYSDATETIME(), 1, 0),
    (NEWID(), '4x400M Relay', 2, 2, SYSDATETIME(), 1, 0),
    (NEWID(), '4x800M Relay', 2, 3, SYSDATETIME(), 1, 0),

    (NEWID(), '4x100M Relay', 2, 1, SYSDATETIME(), 2, 0),
    (NEWID(), '4x200M Relay', 2, 2, SYSDATETIME(), 2, 0),
    (NEWID(), '4x400M Relay', 2, 3, SYSDATETIME(), 2, 0),
    (NEWID(), '4x800M Relay', 2, 4, SYSDATETIME(), 2, 0),
    (NEWID(), '4x1600M Relay', 2, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Sprint Medley', 2, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Distance Medley', 2, 7, SYSDATETIME(), 2, 0),
    (NEWID(), '4x110M Shuttle Hurdles', 2, 8, SYSDATETIME(), 2, 0),
    (NEWID(), '4x300M Shuttle Hurdles', 2, 9, SYSDATETIME(), 2, 0),
    (NEWID(), '4x100M Thrower Relay', 2, 10, SYSDATETIME(), 2, 0);

INSERT INTO
    [FieldEvents]
([Id], [Name], [Gender], [SortOrder], [DateCreated], [Environment], [Deleted])
VALUES
    (NEWID(), 'High Jump', 1, 1, SYSDATETIME(), 1, 0),
    (NEWID(), 'Long Jump', 1, 2, SYSDATETIME(), 1, 0),
    (NEWID(), 'Triple Jump', 1, 3, SYSDATETIME(), 1, 0),
    (NEWID(), 'Pole Vault', 1, 4, SYSDATETIME(), 1, 0),
    (NEWID(), 'Shot Put', 1, 5, SYSDATETIME(), 1, 0),
    (NEWID(), 'Weight Throw', 1, 6, SYSDATETIME(), 1, 0),
    (NEWID(), 'Seated Shot Put', 1, 7, SYSDATETIME(), 1, 0),

    (NEWID(), 'High Jump', 1, 1, SYSDATETIME(), 2, 0),
    (NEWID(), 'Long Jump', 1, 2, SYSDATETIME(), 2, 0),
    (NEWID(), 'Triple Jump', 1, 3, SYSDATETIME(), 2, 0),
    (NEWID(), 'Pole Vault', 1, 4, SYSDATETIME(), 2, 0),
    (NEWID(), 'Shot Put', 1, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Discus', 1, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Hammer Throw', 1, 7, SYSDATETIME(), 2, 0),
    (NEWID(), 'Seated Shot Put', 1, 8, SYSDATETIME(), 2, 0),
       
    (NEWID(), 'High Jump', 2, 1, SYSDATETIME(), 1, 0),
    (NEWID(), 'Long Jump', 2, 2, SYSDATETIME(), 1, 0),
    (NEWID(), 'Triple Jump', 2, 3, SYSDATETIME(), 1, 0),
    (NEWID(), 'Pole Vault', 2, 4, SYSDATETIME(), 1, 0),
    (NEWID(), 'Shot Put', 2, 5, SYSDATETIME(), 1, 0),
    (NEWID(), 'Weight Throw', 2, 6, SYSDATETIME(), 1, 0),
    (NEWID(), 'Seated Shot Put', 2, 7, SYSDATETIME(), 1, 0),
       
    (NEWID(), 'High Jump', 2, 1, SYSDATETIME(), 2, 0),
    (NEWID(), 'Long Jump', 2, 2, SYSDATETIME(), 2, 0),
    (NEWID(), 'Triple Jump', 2, 3, SYSDATETIME(), 2, 0),
    (NEWID(), 'Pole Vault', 2, 4, SYSDATETIME(), 2, 0),
    (NEWID(), 'Shot Put', 2, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Discus', 2, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Hammer Throw', 2, 7, SYSDATETIME(), 2, 0),
    (NEWID(), 'Seated Shot Put', 2, 8, SYSDATETIME(), 2, 0);

INSERT INTO
    [FieldRelayEvents]
([Id], [Name], [Gender], [SortOrder], [DateCreated], [Environment], [Deleted])
VALUES
    (NEWID(), 'High Jump Relay', 1, 1, SYSDATETIME(), 2, 0),
    (NEWID(), 'Long Jump Relay', 1, 2, SYSDATETIME(), 2, 0),
    (NEWID(), 'Triple Jump Relay', 1, 3, SYSDATETIME(), 2, 0),
    (NEWID(), 'Pole Vault Relay', 1, 4, SYSDATETIME(), 2, 0),
    (NEWID(), 'Shot Put Relay', 1, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Discus Relay', 1, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Hammer Throw Relay', 1, 7, SYSDATETIME(), 2, 0),

    (NEWID(), 'High Jump Relay', 2, 1, SYSDATETIME(), 2, 0),
    (NEWID(), 'Long Jump Relay', 2, 2, SYSDATETIME(), 2, 0),
    (NEWID(), 'Triple Jump Relay', 2, 3, SYSDATETIME(), 2, 0),
    (NEWID(), 'Pole Vault Relay', 2, 4, SYSDATETIME(), 2, 0),
    (NEWID(), 'Shot Put Relay', 2, 5, SYSDATETIME(), 2, 0),
    (NEWID(), 'Discus Relay', 2, 6, SYSDATETIME(), 2, 0),
    (NEWID(), 'Hammer Throw Relay', 2, 7, SYSDATETIME(), 2, 0);
