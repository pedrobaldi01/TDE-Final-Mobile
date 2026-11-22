-- O ALTER pode ser repetido após uma interrupção.
SET @ddl = IF(EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema=DATABASE()
    AND table_name='workout_plan_items' AND column_name='suggested_weight_kg'), 'SELECT 1',
    'ALTER TABLE workout_plan_items ADD COLUMN suggested_weight_kg DECIMAL(6,2) NULL');
PREPARE mobile_ddl FROM @ddl;
EXECUTE mobile_ddl;
DEALLOCATE PREPARE mobile_ddl;

CREATE TABLE IF NOT EXISTS student_measurements (
    student_id CHAR(36) CHARACTER SET ascii COLLATE ascii_bin NOT NULL PRIMARY KEY,
    first_name VARCHAR(120) NOT NULL, last_name VARCHAR(120), body_weight_kg DECIMAL(6,2), height_cm INT,
    FOREIGN KEY(student_id) REFERENCES users(id) ON DELETE CASCADE
) ENGINE=InnoDB;
INSERT IGNORE INTO schema_migrations (version,name) VALUES (11,'mobile_final');
