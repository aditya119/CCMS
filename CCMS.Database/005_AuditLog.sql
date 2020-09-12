-------------------------------------------------------------------------
-- Create AuditLog table and procedure
-------------------------------------------------------------------------

CREATE TABLE audit_log (
    audit_log_id     NUMBER
        GENERATED ALWAYS AS IDENTITY ( START WITH 1 INCREMENT BY 1 ),
    updated_table    VARCHAR2(4000),
    updated_row_pk   NUMBER(25),
    updated_column   VARCHAR2(4000),
    old_value        VARCHAR2(4000),
    new_value        VARCHAR2(4000),
    updated_by       NUMBER(10) NOT NULL,
    log_timestamp    TIMESTAMP DEFAULT systimestamp,
    CONSTRAINT audit_log_pk PRIMARY KEY ( audit_log_id ),
    CONSTRAINT fk_update_by_user FOREIGN KEY ( updated_by )
        REFERENCES app_users ( user_id )
);
/

create or replace procedure p_audit_log (
    pi_table_name audit_log.updated_table%type,
    pi_row_pk     audit_log.updated_row_pk%type,
    pi_col_name   audit_log.updated_column%type,
    pi_old_value  audit_log.old_value%type,
    pi_new_value  audit_log.new_value%type,
    pi_updated_by audit_log.updated_by%type
) as
begin
    insert into audit_log (
        updated_table,
        updated_row_pk,
        updated_column,
        old_value,
        new_value,
        updated_by
    ) values (
        pi_table_name,
        pi_row_pk,
        pi_col_name,
        pi_old_value,
        pi_new_value,
        pi_updated_by
    );
exception
    when others then
        raise_application_error(
            -20001,
            'p_audit_log - pi_table_name: ' || pi_table_name
            || '; pi_row_pk: ' || pi_row_pk
            || '; pi_col_name: ' || pi_col_name
            || '; pi_old_value: ' || pi_old_value
            || '; pi_new_value: ' || pi_new_value
            || '; pi_updated_by: ' || pi_updated_by || chr(10)
            || sqlerrm);
end p_audit_log;
/