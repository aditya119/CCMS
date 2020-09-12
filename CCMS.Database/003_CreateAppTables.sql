-------------------------------------------------------------------------
-- Create new APP tables
-------------------------------------------------------------------------

-- Todo: Add comments to columns
CREATE TABLE attachments ( -- attachments will be stored on server
    attachment_id   NUMBER(10)
        GENERATED BY DEFAULT AS IDENTITY ( START WITH 10000 INCREMENT BY 1 ),
    filename        VARCHAR2(1000) NOT NULL,
    date_created    DATE DEFAULT SYSDATE,
    deleted         DATE DEFAULT NULL,
    CONSTRAINT attachments_pk PRIMARY KEY ( attachment_id )
);

CREATE TABLE court_cases (
    case_id          NUMBER(10)
        GENERATED BY DEFAULT AS IDENTITY ( START WITH 10000 INCREMENT BY 1 ),
    case_number      VARCHAR2(1000) NOT NULL,-- ensure proc stores upper case always
    appeal_number    NUMBER(10) DEFAULT 0,
    case_type_id     NUMBER(10) NOT NULL,
    court_id         NUMBER(10) NOT NULL,
    location_id      NUMBER(10) NOT NULL,
    lawyer_id        NUMBER(10) NOT NULL,
    case_status      NUMBER(10) NOT NULL,
    last_update_by   NUMBER(10) NOT NULL,
    created_by       NUMBER(10) NOT NULL,
    date_created     DATE DEFAULT SYSDATE,
    deleted          DATE DEFAULT NULL,
    CONSTRAINT cases_pk PRIMARY KEY ( case_id ),
    CONSTRAINT fk_case_type FOREIGN KEY ( case_type_id )
        REFERENCES case_types ( case_type_id ),
    CONSTRAINT fk_court FOREIGN KEY ( court_id )
        REFERENCES courts ( court_id ),
    CONSTRAINT fk_location FOREIGN KEY ( location_id )
        REFERENCES locations ( location_id ),
    CONSTRAINT fk_lawyer FOREIGN KEY ( lawyer_id )
        REFERENCES lawyers ( lawyer_id ),
    CONSTRAINT fk_case_status FOREIGN KEY ( case_status )
        REFERENCES proceeding_decisions ( proceeding_decision_id ),
    CONSTRAINT fk_case_updated_by FOREIGN KEY ( last_update_by )
        REFERENCES app_users ( user_id ),
    CONSTRAINT fk_case_created_by FOREIGN KEY ( created_by )
        REFERENCES app_users ( user_id ),
    CONSTRAINT unique_case UNIQUE ( case_number, appeal_number )
);

CREATE TABLE case_dates (
    case_date_id         NUMBER(10)
        GENERATED BY DEFAULT AS IDENTITY ( START WITH 10000 INCREMENT BY 1 ),
    case_id              NUMBER(10) NOT NULL,
    case_filed_on        DATE,
    notice_received_on   DATE,
    first_hearing_on     DATE,
    last_update_by       NUMBER(10) NOT NULL,
    deleted              DATE DEFAULT NULL,
    CONSTRAINT case_dates_pk PRIMARY KEY ( case_date_id ),
    CONSTRAINT fk_courtcase_dates FOREIGN KEY ( case_id )
        REFERENCES court_cases ( case_id ),
    CONSTRAINT fk_dates_updated_by FOREIGN KEY ( last_update_by )
        REFERENCES app_users ( user_id )
);

CREATE TABLE case_actors (
    case_actor_id   NUMBER(10)
        GENERATED BY DEFAULT AS IDENTITY ( START WITH 10000 INCREMENT BY 1 ),
    case_id         NUMBER(10) NOT NULL,
    actor_type_id   NUMBER(10) NOT NULL,-- 1 PETI,2 RESP
    actor_name      VARCHAR2(1000) NOT NULL,
    actor_address   VARCHAR2(4000),
    actor_email     VARCHAR2(1000),
    actor_phone     VARCHAR2(20),
    detail_file     NUMBER(10),
    last_update_by  NUMBER(10) NOT NULL,
    deleted         DATE DEFAULT NULL,
    CONSTRAINT case_actors_pk PRIMARY KEY ( case_actor_id ),
    CONSTRAINT fk_courtcase_actors FOREIGN KEY ( case_id )
        REFERENCES court_cases ( case_id ),
    CONSTRAINT fk_actor_type FOREIGN KEY ( actor_type_id )
        REFERENCES actor_types ( actor_type_id ),
    CONSTRAINT fk_actor_attachment FOREIGN KEY ( detail_file )
        REFERENCES attachments ( attachment_id ),
    CONSTRAINT fk_actors_updated_by FOREIGN KEY ( last_update_by )
        REFERENCES app_users ( user_id ),
    CONSTRAINT unique_case_actors UNIQUE ( case_id,actor_type_id )
);

CREATE TABLE case_proceedings (
    case_proceeding_id    NUMBER(10)
        GENERATED BY DEFAULT AS IDENTITY ( START WITH 10000 INCREMENT BY 1 ),
    case_id               NUMBER(10) NOT NULL,
    proceeding_date       DATE NOT NULL,
    proceeding_decision   NUMBER(10) NOT NULL,
    next_hearing_on       DATE,
    judgement_file        NUMBER(10) NOT NULL,
    assigned_to           NUMBER(10) NOT NULL,
    last_update_by        NUMBER(10) NOT NULL,
    deleted               DATE DEFAULT NULL,
    CONSTRAINT case_proceedings_pk PRIMARY KEY ( case_proceeding_id ),
    CONSTRAINT fk_courtcase_proceedings FOREIGN KEY ( case_id )
        REFERENCES court_cases ( case_id ),
    CONSTRAINT fk_proceeding_decisions FOREIGN KEY ( proceeding_decision )
        REFERENCES proceeding_decisions ( proceeding_decision_id ),
    CONSTRAINT fk_proceeding_attachment FOREIGN KEY ( judgement_file )
        REFERENCES attachments ( attachment_id ),
    CONSTRAINT fk_assigned_user FOREIGN KEY ( assigned_to )
        REFERENCES app_users ( user_id ),
    CONSTRAINT fk_proceeding_updated_by FOREIGN KEY ( last_update_by )
        REFERENCES app_users ( user_id )
);