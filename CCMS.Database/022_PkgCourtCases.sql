-------------------------------------------------------------------------
-- PKG_COURT_CASES: Perform CRUD operations on COURT_CASES
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_court_cases IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_details (
        pi_case_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_exists_case_number (
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        po_case_id          OUT court_cases.case_id%type,
        po_deleted          OUT court_cases.deleted%type
    );

    PROCEDURE p_exists_case_id (
        pi_case_id          IN court_cases.case_id%type,
        po_case_number      OUT court_cases.case_number%type,
        po_appeal_number    OUT court_cases.appeal_number%type,
        po_deleted          OUT court_cases.deleted%type
    );
    
    PROCEDURE p_get_case_status (
        pi_case_id          IN court_cases.case_id%type,
        po_status_id        OUT court_cases.case_status%type,
        po_status           OUT proceeding_decisions.proceeding_decision_name%type
    );

    PROCEDURE p_add_new_case (
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        pi_case_type_id     IN court_cases.case_type_id%type,
        pi_court_id         IN court_cases.court_id%type,
        pi_location_id      IN court_cases.location_id%type,
        pi_lawyer_id        IN court_cases.lawyer_id%type,
        pi_action_by        IN court_cases.created_by%type,
        po_case_id          OUT court_cases.case_id%type
    );

    PROCEDURE p_update_case (
        pi_case_id          IN court_cases.case_id%type,
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        pi_case_type_id     IN court_cases.case_type_id%type,
        pi_court_id         IN court_cases.court_id%type,
        pi_location_id      IN court_cases.location_id%type,
        pi_lawyer_id        IN court_cases.lawyer_id%type,
        pi_update_by        IN court_cases.last_update_by%type
    );

    PROCEDURE p_delete_case (
        pi_case_id  IN court_cases.case_id%type
    );

END pkg_court_cases;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_court_cases IS
-------------------------------------------------------------------------
    PROCEDURE p_get_case_details (
        pi_case_id  IN court_cases.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                case_number,
                appeal_number,
                case_type_id,
                court_id,
                location_id,
                lawyer_id,
                case_status,
                deleted
            from
                court_cases
            where
                case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_case_details - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_case_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_case_number (
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        po_case_id          OUT court_cases.case_id%type,
        po_deleted          OUT court_cases.deleted%type
    ) IS
    BEGIN
        select
            case_id,
            deleted
        into
            po_case_id,
            po_deleted
        from
            court_cases
        where   case_number = pi_case_number
            and appeal_number = pi_appeal_number;
    EXCEPTION
        when no_data_found then
            po_case_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_case_number - pi_case_number: ' || pi_case_number
                || '; pi_appeal_number: ' || pi_appeal_number
                || chr(10) || sqlerrm);
    END p_exists_case_number;
-------------------------------------------------------------------------
    PROCEDURE p_exists_case_id (
        pi_case_id          IN court_cases.case_id%type,
        po_case_number      OUT court_cases.case_number%type,
        po_appeal_number    OUT court_cases.appeal_number%type,
        po_deleted          OUT court_cases.deleted%type
    ) IS
    BEGIN
        select
            case_number,
            appeal_number,
            deleted
        into
            po_case_number,
            po_appeal_number,
            po_deleted
        from
            court_cases
        where
            case_id = pi_case_id;
    EXCEPTION
        when no_data_found then
            po_case_number := null;
            po_appeal_number := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_case_id - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_exists_case_id;
-------------------------------------------------------------------------
    PROCEDURE p_get_case_status (
        pi_case_id          IN court_cases.case_id%type,
        po_status_id        OUT court_cases.case_status%type,
        po_status           OUT proceeding_decisions.proceeding_decision_name%type
    ) IS
    BEGIN
        select
            cc.case_status,
            pd.proceeding_decision_name
        into
            po_status_id,
            po_status
        from
            court_cases cc,
            proceeding_decisions pd
        where   cc.case_status = pd.proceeding_decision_id
            and case_id = pi_case_id;
    EXCEPTION
        when no_data_found then
            po_status_id := -1;
            po_status := null;
        when others then
            raise_application_error(
                -20001,
                'p_get_case_status - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_case_status;
-------------------------------------------------------------------------
    PROCEDURE p_add_new_case (
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        pi_case_type_id     IN court_cases.case_type_id%type,
        pi_court_id         IN court_cases.court_id%type,
        pi_location_id      IN court_cases.location_id%type,
        pi_lawyer_id        IN court_cases.lawyer_id%type,
        pi_action_by        IN court_cases.created_by%type,
        po_case_id          OUT court_cases.case_id%type
    ) IS
        v_case_id   court_cases.case_id%type;
        v_deleted   court_cases.deleted%type;
    BEGIN
        p_exists_case_number (pi_case_number, pi_appeal_number, v_case_id, v_deleted);
        if v_case_id is not null then
            if v_deleted is not null then
                update court_cases
                set deleted = null
                where case_id = v_case_id;
                update case_actors
                set deleted = null
                where case_id = v_case_id;
                update case_dates
                set deleted = null
                where case_id = v_case_id;
                update case_proceedings
                set deleted = null
                where case_id = v_case_id;
                p_update_case (v_case_id, pi_case_number, pi_appeal_number, pi_case_type_id, pi_court_id, pi_location_id, pi_lawyer_id, pi_action_by);
            else
                po_case_id := -1;
            end if;
            return;
        end if;
        
        insert into court_cases (
            case_number,
            appeal_number,
            case_type_id,
            court_id,
            location_id,
            lawyer_id,
            case_status,
            last_update_by,
            created_by
        ) values (
            pi_case_number,
            pi_appeal_number,
            pi_case_type_id,
            pi_court_id,
            pi_location_id,
            pi_lawyer_id,
            0,
            pi_action_by,
            pi_action_by
        )
        returning
            case_id
        into
            po_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_add_new_case - pi_case_number: ' || pi_case_number
                || '; pi_case_type_id: ' || pi_case_type_id
                || '; pi_court_id: ' || pi_court_id
                || '; pi_location_id: ' || pi_location_id
                || '; pi_lawyer_id: ' || pi_lawyer_id
                || '; pi_action_by: ' || pi_action_by
                || chr(10) || sqlerrm);
    END p_add_new_case;
-------------------------------------------------------------------------
    PROCEDURE p_update_case (
        pi_case_id          IN court_cases.case_id%type,
        pi_case_number      IN court_cases.case_number%type,
        pi_appeal_number    IN court_cases.appeal_number%type,
        pi_case_type_id     IN court_cases.case_type_id%type,
        pi_court_id         IN court_cases.court_id%type,
        pi_location_id      IN court_cases.location_id%type,
        pi_lawyer_id        IN court_cases.lawyer_id%type,
        pi_update_by        IN court_cases.last_update_by%type
    ) IS
    BEGIN
        update court_cases
        set case_number = pi_case_number,
            appeal_number = pi_appeal_number,
            case_type_id = pi_case_type_id,
            court_id = pi_court_id,
            location_id = pi_location_id,
            lawyer_id = pi_lawyer_id,
            last_update_by = pi_update_by
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case - pi_case_id: ' || pi_case_id
                || '; pi_case_number: ' || pi_case_number
                || '; pi_appeal_number: ' || pi_appeal_number
                || '; pi_case_type_id: ' || pi_case_type_id
                || '; pi_court_id: ' || pi_court_id
                || '; pi_location_id: ' || pi_location_id
                || '; pi_lawyer_id: ' || pi_lawyer_id
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_case;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case (
        pi_case_id  IN court_cases.case_id%type
    ) IS
    BEGIN
        update court_cases
        set deleted = sysdate
        where case_id = pi_case_id;
        
        pkg_case_dates.p_delete_case_dates(pi_case_id);
        
        pkg_case_actors.p_delete_case_actors(pi_case_id);
        
        pkg_case_proceedings.p_delete_case_proceedings(pi_case_id);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_delete_case;
-------------------------------------------------------------------------
END pkg_court_cases;
/
-------------------------------------------------------------------------