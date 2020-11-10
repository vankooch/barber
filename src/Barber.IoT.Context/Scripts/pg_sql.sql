--
-- Barber IoT PostgreSQL Initial
--
SET client_encoding = 'UTF8';

--
-- Roles
--
CREATE ROLE barber_iot;
--ALTER ROLE barber_iot WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB LOGIN NOREPLICATION NOBYPASSRLS PASSWORD 'md5----' VALID UNTIL 'infinity';
CREATE ROLE barber_iot_group;
ALTER ROLE barber_iot_group WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB NOLOGIN NOREPLICATION NOBYPASSRLS VALID UNTIL 'infinity';

CREATE ROLE barber_iot_group_stage;
ALTER ROLE barber_iot_group_stage WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB NOLOGIN NOREPLICATION NOBYPASSRLS VALID UNTIL 'infinity';
CREATE ROLE barber_iot_stage;
--ALTER ROLE barber_iot_stage WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB LOGIN NOREPLICATION NOBYPASSRLS PASSWORD 'md5--' VALID UNTIL 'infinity';

--
-- Role memberships
--
GRANT barber_iot_group TO barber_iot GRANTED BY pgsql;
GRANT barber_iot_group_stage TO barber_iot_stage GRANTED BY pgsql;

--
-- Database
--
CREATE DATABASE barber_iot  WITH OWNER = barber_iot_group ENCODING = 'UTF8';
GRANT CONNECT, TEMPORARY ON DATABASE barber_iot TO public;
GRANT ALL ON DATABASE barber_iot TO barber_iot_group;

CREATE DATABASE barber_iot_stage WITH OWNER = barber_iot_group_stage ENCODING = 'UTF8';
GRANT CONNECT, TEMPORARY ON DATABASE barber_iot_stage TO public;
GRANT ALL ON DATABASE barber_iot_stage TO barber_iot_group_stage;

-- REASSIGN OWNED BY barber_iot_group TO barber_iot_stage;
-- REASSIGN OWNED BY barber_iot TO barber_iot_group;
-- ALTER DATABASE barber_iot OWNER TO barber_iot_group;
-- GRANT ALL ON DATABASE barber_iot TO barber_iot_group;